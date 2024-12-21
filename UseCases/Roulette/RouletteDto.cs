namespace CSharpClicker.Web.UseCases.Roulette
{
    public record RouletteDto
    {
        public long CurrentScore { get; init; }

        public int WinnerIndex { get; init; }
    }
}
