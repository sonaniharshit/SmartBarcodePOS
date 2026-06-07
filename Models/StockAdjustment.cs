namespace SmartBarcodePOS_Pro.Models;

public class StockAdjustment
{
    public int StockAdjustmentId { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Qty { get; set; }
    public string AdjustmentType { get; set; } = "Manual Correction";
    public string? Reason { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
