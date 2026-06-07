namespace SmartBarcodePOS_Pro.Models;

public class AppSetting
{
    public int AppSettingId { get; set; }
    public string ShopName { get; set; } = "Smart POS Store";
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? GstNumber { get; set; }
    public string CurrencySymbol { get; set; } = "₹";
    public string FooterMessage { get; set; } = "Thank you, visit again!";
    public string PrintSize { get; set; } = "80mm";
}
