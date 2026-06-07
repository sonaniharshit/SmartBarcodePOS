namespace SmartBarcodePOS_Pro.Models;

public class Purchase
{
    public int PurchaseId { get; set; }
    public string PurchaseNo { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public List<PurchaseItem> Items { get; set; } = new();
}
