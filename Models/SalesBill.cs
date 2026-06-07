namespace SmartBarcodePOS_Pro.Models;

public class SalesBill
{
    public int SalesBillId { get; set; }
    public string BillNo { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public decimal SubTotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public List<SalesBillItem> Items { get; set; } = new();
}
