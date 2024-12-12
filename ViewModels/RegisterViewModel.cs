using System.ComponentModel.DataAnnotations;

namespace CSharpClicker.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        [StringLength(30)]
        public string UserName { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; init; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; init; }

        public string? CaptchaToken { get; set; }

        public string CaptchaKey { get; init; }
    }
}
