namespace SmartBarcodePOS_Pro.Models;

public class PurchaseItem
{
    public int PurchaseItemId { get; set; }
    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Qty { get; set; }
    public decimal CostPrice { get; set; }
    public decimal Total { get; set; }
}
