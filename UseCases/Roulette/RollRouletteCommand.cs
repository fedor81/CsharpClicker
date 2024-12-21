using MediatR;

namespace CSharpClicker.Web.UseCases.Roulette;

public record RollRouletteCommand(int Bet) : IRequest<RouletteDto>;
