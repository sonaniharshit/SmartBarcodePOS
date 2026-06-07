using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class AppSettingViewModel
{
    public int AppSettingId { get; set; }

    [Required(ErrorMessage = "Business name is required")]
    [Display(Name = "Business Name")]
    public string ShopName { get; set; } = string.Empty;

    [Display(Name = "Business Address")]
    public string? Address { get; set; }

    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [Display(Name = "GST Number")]
    public string? GstNumber { get; set; }

    [Required(ErrorMessage = "Currency symbol is required")]
    [Display(Name = "Currency Symbol")]
    public string CurrencySymbol { get; set; } = "₹";
}
