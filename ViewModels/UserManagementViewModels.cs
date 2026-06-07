using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class UserListViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateUserViewModel
{
    [Required(ErrorMessage = "Please enter full name")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter email")]
    [EmailAddress(ErrorMessage = "Please enter valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Please enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter confirm password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select role")]
    [Display(Name = "User Role")]
    public string RoleName { get; set; } = "SubAdmin";

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter full name")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter email")]
    [EmailAddress(ErrorMessage = "Please enter valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Please select role")]
    [Display(Name = "User Role")]
    public string RoleName { get; set; } = "SubAdmin";

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class AdminResetPasswordViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter new password")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter confirm password")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
