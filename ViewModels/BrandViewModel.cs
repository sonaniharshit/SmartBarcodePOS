using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class BrandViewModel
{
    public int BrandId { get; set; }

    [Required(ErrorMessage = "Please enter brand name")]
    [Display(Name = "Brand Name")]
    public string BrandName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
