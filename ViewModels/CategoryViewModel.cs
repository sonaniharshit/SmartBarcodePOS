using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class CategoryViewModel
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Please select brand name")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select brand")]
    [Display(Name = "Brand")]
    public int BrandId { get; set; }

    [Required(ErrorMessage = "Please enter category name")]
    [StringLength(100, ErrorMessage = "Category name cannot be more than 100 characters")]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> Brands { get; set; } = new();
}
