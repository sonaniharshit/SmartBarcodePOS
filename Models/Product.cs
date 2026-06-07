using System.ComponentModel.DataAnnotations;

namespace SmartBarcodePOS_Pro.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required]
    public string SKU { get; set; } = string.Empty;

    [Required]
    public string ProductName { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }
    public decimal? CostPrice { get; set; }

    public int StockQty { get; set; }
    public int MinimumStockAlert { get; set; } = 5;

    public string? BarcodeImage { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
