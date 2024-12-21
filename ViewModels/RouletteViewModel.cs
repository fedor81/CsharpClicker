using CSharpClicker.Web.UseCases.GetCurrentUser;

namespace CSharpClicker.Web.ViewModels
{
    public class RouletteViewModel
    {
        public UserDto User { get; init; }

        public int MinBet { get; init; } = 100;
    }
}
