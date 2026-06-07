using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public class SettingService : ISettingService
{
    private readonly ApplicationDbContext _context;

    public SettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettingViewModel> GetBusinessSettingsAsync()
    {
        var setting = await GetOrCreateSettingAsync();

        return new AppSettingViewModel
        {
            AppSettingId = setting.AppSettingId,
            ShopName = setting.ShopName,
            Address = setting.Address,
            Phone = setting.Phone,
            GstNumber = setting.GstNumber,
            CurrencySymbol = setting.CurrencySymbol
        };
    }

    public async Task SaveBusinessSettingsAsync(AppSettingViewModel model)
    {
        var setting = await GetOrCreateSettingAsync();

        setting.ShopName = model.ShopName.Trim();
        setting.Address = model.Address?.Trim();
        setting.Phone = model.Phone?.Trim();
        setting.GstNumber = model.GstNumber?.Trim();
        setting.CurrencySymbol = model.CurrencySymbol.Trim();

        await _context.SaveChangesAsync();
    }

    private async Task<AppSetting> GetOrCreateSettingAsync()
    {
        var setting = await _context.AppSettings.FirstOrDefaultAsync();

        if (setting != null)
            return setting;

        setting = new AppSetting
        {
            ShopName = "Smart POS Store",
            Address = "Surat, Gujarat",
            Phone = "9876543210",
            GstNumber = "",
            CurrencySymbol = "₹",
            FooterMessage = "Thank you, visit again!",
            PrintSize = "80mm"
        };

        _context.AppSettings.Add(setting);
        await _context.SaveChangesAsync();

        return setting;
    }
}
