using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public interface ISettingService
{
    Task<AppSettingViewModel> GetBusinessSettingsAsync();
    Task SaveBusinessSettingsAsync(AppSettingViewModel model);
}
