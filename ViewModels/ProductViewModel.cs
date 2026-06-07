using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.ViewModels;

public class ProductViewModel
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter product name")]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select category")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select category")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Please select brand")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select brand")]
    [Display(Name = "Brand")]
    public int? BrandId { get; set; }

    public string? Description { get; set; }

    [Required]
    [Range(0.01, 999999)]
    [Display(Name = "Selling Price")]
    public decimal Price { get; set; }

    [Display(Name = "Cost Price")]
    public decimal? CostPrice { get; set; }

    [Required]
    [Range(0, 999999)]
    [Display(Name = "Stock Quantity")]
    public int StockQty { get; set; }

    [Required]
    [Range(0, 999999)]
    [Display(Name = "Minimum Stock Alert")]
    public int MinimumStockAlert { get; set; } = 5;

    public bool IsActive { get; set; } = true;
    public string? BarcodeImage { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Brands { get; set; } = new();
}
