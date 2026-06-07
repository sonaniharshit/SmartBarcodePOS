using SmartBarcodePOS_Pro.Models;

namespace SmartBarcodePOS_Pro.ViewModels;

public class SalesReportViewModel
{
    public List<SalesBill> DailyBills { get; set; } = new();
    public List<SalesBill> MonthlyBills { get; set; } = new();
    public decimal DailyTotal { get; set; }
    public decimal MonthlyTotal { get; set; }
}

public class TopSellingProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public decimal TotalAmount { get; set; }
}
