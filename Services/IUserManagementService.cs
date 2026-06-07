using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public interface IUserManagementService
{
    Task<List<UserListViewModel>> GetUsersAsync();
    CreateUserViewModel GetCreateModel();
    Task<EditUserViewModel?> GetEditModelAsync(string id);
    Task<(ServiceResult Result, Dictionary<string, List<string>> Errors)> CreateUserAsync(CreateUserViewModel model);
    Task<(ServiceResult Result, Dictionary<string, List<string>> Errors)> UpdateUserAsync(EditUserViewModel model);
    Task<ServiceResult> ToggleStatusAsync(string id);
    Task<(AdminResetPasswordViewModel? Model, string? UserName, string? Email)> GetResetPasswordModelAsync(string id);
    Task<(ServiceResult Result, Dictionary<string, List<string>> Errors, string? UserName, string? Email)> ResetPasswordAsync(AdminResetPasswordViewModel model);
    Task<ServiceResult> DeleteUserAsync(string id);
}
