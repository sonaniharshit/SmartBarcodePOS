using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<ProductViewModel> GetCreateModelAsync();
    Task<ProductViewModel?> GetEditModelAsync(int id);
    Task<Product> CreateAsync(ProductViewModel model);
    Task<bool> UpdateAsync(ProductViewModel model);
    Task<bool> DeleteAsync(int id);
}
