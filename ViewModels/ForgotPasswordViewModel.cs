using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Please enter email address")]
    [EmailAddress(ErrorMessage = "Please enter valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;
}
