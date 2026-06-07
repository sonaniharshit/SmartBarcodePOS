using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetActiveAsync();
    Task<List<Category>> GetActiveByBrandAsync(int brandId);
    Task<Category?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CategoryViewModel model);
    Task<(bool Success, string Message)> UpdateAsync(CategoryViewModel model);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<(bool Success, string Message)> ToggleStatusAsync(int id);
}
