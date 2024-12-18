using CSharpClicker.Web.DomainServices;
using CSharpClicker.Web.Infrastructure.Abstractions;
using CSharpClicker.Web.UseCases.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CSharpClicker.Web.UseCases.AddPoints;

public class AddPointsCommandHandler : IRequestHandler<AddPointsCommand, ScoreDto>
{
    private readonly ICurrentUserAccessor currentUserAccessor;
    private readonly IAppDbContext appDbContext;

    public AddPointsCommandHandler(ICurrentUserAccessor currentUserAccessor, IAppDbContext appDbContext)
    {
        this.currentUserAccessor = currentUserAccessor;
        this.appDbContext = appDbContext;
    }

    public async Task<ScoreDto> Handle(AddPointsCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserAccessor.GetCurrentUserId();
        var user = await appDbContext.ApplicationUsers
            .Include(user => user.UserBoosts)
            .ThenInclude(ub => ub.Boost)
            .FirstAsync(user => user.Id == userId);

        var profitPerSecond = user.UserBoosts.GetProfit(shouldCalculateAutoBoosts: true);
        var profitPerClick = user.UserBoosts.GetProfit();

        // Ограничение по максимальному заработку на запрос

        var lastRequestTime = user.LastRequestTime;
        var lastRequestTimeDelta = DateTime.UtcNow - lastRequestTime;
        var seconds = (int)Math.Min(lastRequestTimeDelta.TotalSeconds, request.Seconds);

        const int maxClickPerSecond = 7;
        var clicks = Math.Min(request.Clicks, seconds * maxClickPerSecond);

        var autoPoints = profitPerSecond * seconds;
        var clickedPoints = profitPerClick * clicks;

        user.CurrentScore += autoPoints + clickedPoints;
        user.RecordScore += autoPoints + clickedPoints;

        user.LastRequestTime = DateTime.UtcNow;

        await appDbContext.SaveChangesAsync();

        return new ScoreDto
        {
            CurrentScore = user.CurrentScore,
            RecordScore = user.RecordScore,
            ProfitPerClick = profitPerClick,
            ProfitPerSecond = profitPerSecond,
        };
    }
}
