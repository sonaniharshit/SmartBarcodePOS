using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public interface IReportService
{
    Task<SalesReportViewModel> GetSalesReportAsync();
    Task<List<SalesBillItem>> GetProductSalesAsync();
    Task<List<TopSellingProductViewModel>> GetTopSellingProductsAsync();
    Task<List<Product>> GetStockReportAsync();
}
