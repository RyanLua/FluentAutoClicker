using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Fluent_Auto_Clicker.Contracts.Services;
using Fluent_Auto_Clicker.Helpers;

using Microsoft.UI.Xaml;

using Windows.ApplicationModel;

namespace Fluent_Auto_Clicker.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IAlwaysOnTopService _alwaysOnTopService;
    private ElementTheme _elementTheme;
    private string _versionDescription;
    private bool _alwaysOnTop;

    public bool AlwaysOnTop
    {
        get => _alwaysOnTop;
        set => SetProperty(ref _alwaysOnTop, value);
    }

    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public ICommand SwitchAlwaysOnTopCommand
    {

        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IAlwaysOnTopService alwaysOnTopService)
    {
        _themeSelectorService = themeSelectorService;
        _alwaysOnTopService = alwaysOnTopService;
        _alwaysOnTop = _alwaysOnTopService.AlwaysOnTop;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
        SwitchAlwaysOnTopCommand = new RelayCommand<bool>(
            (param) =>
            {
                AlwaysOnTop = param;
            });
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
