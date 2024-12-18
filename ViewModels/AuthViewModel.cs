using System.ComponentModel.DataAnnotations;

namespace CSharpClicker.Web.ViewModels;

public class AuthViewModel
{
    [Required]
    [MaxLength(30)]
    [Display(Name = "Имя пользователя")]
    public string UserName { get; init; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; init; }

    [Display(Name = "Запомнить меня")]
    public bool RememberMe { get; init; }
}
