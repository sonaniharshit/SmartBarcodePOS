using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class CustomerViewModel
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Customer name is required")]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required")]
    [Display(Name = "Mobile Number")]
    public string MobileNo { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Display(Name = "Loyalty Points")]
    public int LoyaltyPoints { get; set; }

    public bool IsActive { get; set; } = true;
}
