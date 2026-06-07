namespace SmartBarcodePOS_Pro.Models;

public class SalesReturn
{
    public int SalesReturnId { get; set; }
    public int SalesBillId { get; set; }
    public SalesBill? SalesBill { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Qty { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
