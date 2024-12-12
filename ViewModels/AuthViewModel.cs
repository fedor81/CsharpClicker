using System.ComponentModel.DataAnnotations;

namespace CSharpClicker.Web.ViewModels;

public class AuthViewModel
{
    [Required]
    public string UserName { get; init; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; }

    public bool RememberMe { get; set; }
}
