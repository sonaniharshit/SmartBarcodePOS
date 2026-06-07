using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage ="Please enter email address")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage ="Please enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
