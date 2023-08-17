using Fluent_Auto_Clicker.Contracts.Services;
using Fluent_Auto_Clicker.Helpers;

using Microsoft.UI.Xaml;
using Param_RootNamespace.Helpers;

namespace Fluent_Auto_Clicker.Services;

public class AlwaysOnTopService : IAlwaysOnTopService
{
    private const string SettingsKey = "AlwaysOnTopEnabled";

    public bool AlwaysOnTop { get; set; } = true;

    private readonly ILocalSettingsService _localSettingsService;

    public AlwaysOnTopService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        AlwaysOnTop = await LoadAlwaysOnTopSettingAsync();
        await Task.CompletedTask;
    }

    public async Task SetAlwaysOnTopAsync(bool topmost)
    {
        AlwaysOnTop = topmost;

        await SetRequestedAlwaysOnTopAsync();
        await SaveAlwaysOnTopSettingAsync(AlwaysOnTop);
    }

    public async Task SetRequestedAlwaysOnTopAsync()
    {
        if (App.MainWindow is Window mainWindow)
        {
            mainWindow.SetIsAlwaysOnTop(AlwaysOnTop);
        }

        await Task.CompletedTask;
    }

    private async Task<bool> LoadAlwaysOnTopSettingAsync()
    {
        return await _localSettingsService.ReadSettingAsync<bool>(SettingsKey);
    }

    private async Task SaveAlwaysOnTopSettingAsync(bool alwaysOnTopEnabled)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey, alwaysOnTopEnabled);
    }
}
