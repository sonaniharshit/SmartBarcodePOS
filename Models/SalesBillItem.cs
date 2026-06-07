namespace SmartBarcodePOS_Pro.Models;

public class SalesBillItem
{
    public int SalesBillItemId { get; set; }
    public int SalesBillId { get; set; }
    public SalesBill? SalesBill { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Qty { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
}
