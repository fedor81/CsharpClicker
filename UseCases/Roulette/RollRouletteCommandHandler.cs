using CSharpClicker.Web.Infrastructure.Abstractions;
using CSharpClicker.Web.UseCases.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CSharpClicker.Web.UseCases.Roulette;

public class RollRouletteCommandHandler : IRequestHandler<RollRouletteCommand, RouletteDto>
{
    private readonly RouletteService rouletteService;
    private readonly ICurrentUserAccessor currentUserAccessor;
    private readonly IAppDbContext appDbContext;

    public RollRouletteCommandHandler(RouletteService rouletteService, ICurrentUserAccessor currentUserAccessor, IAppDbContext appDbContext)
    {
        this.rouletteService = rouletteService;
        this.currentUserAccessor = currentUserAccessor;
        this.appDbContext = appDbContext;
    }

    public async Task<RouletteDto> Handle(RollRouletteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserAccessor.GetCurrentUserId();
        var user = await appDbContext.ApplicationUsers
            .FirstAsync(user => user.Id == userId, cancellationToken: cancellationToken);

        if (user.CurrentScore < request.Bet)
        {
            throw new UnauthorizedAccessException("Недостаточно средств для ставки");
        }

        var winnerIndex = await rouletteService.GetWinnerItem();
        var coefficient = (winnerIndex - 1);
        var profit = coefficient * request.Bet;

        user.CurrentScore += profit;

        if (coefficient > 0)
            user.RecordScore += profit;

        await appDbContext.SaveChangesAsync(cancellationToken);

        return new RouletteDto
        {
            CurrentScore = user.CurrentScore,
            WinnerIndex = winnerIndex,
        };
    }
}
