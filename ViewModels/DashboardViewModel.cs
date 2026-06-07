namespace SmartBarcodePOS_Pro.ViewModels;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TodaysSales { get; set; }
    public int TotalBills { get; set; }
    public int LowStockProducts { get; set; }
}
