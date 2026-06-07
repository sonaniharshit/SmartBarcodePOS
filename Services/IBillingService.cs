using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public interface IBillingService
{
    Task<BillingProductViewModel?> GetProductBySkuAsync(string sku);
    Task<(bool Success, string Message, int BillId, string? BillNo)> GenerateBillAsync(GenerateBillViewModel model);
    Task<List<SalesBill>> GetBillsAsync();
    Task<SalesBill?> GetBillAsync(int id);
}
