namespace CSharpClicker.Web.ViewModels
{
    public class RegisterViewModel
    {
        public string UserName { get; init; }

        public string Password { get; init; }

        public string CaptchaToken { get; set; }

        public string CaptchaKey { get; init; } 
    }
}
