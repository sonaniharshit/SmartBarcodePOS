namespace SmartBarcodePOS_Pro.ViewModels;

public class BillingProductViewModel
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQty { get; set; }
}

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Qty { get; set; }
}

public class GenerateBillViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
}
