// Copyright (C) 2024 Ryan Luu
//
// This file is part of Fluent Auto Clicker.
//
// Fluent Auto Clicker is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Fluent Auto Clicker is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Fluent Auto Clicker. If not, see <https://www.gnu.org/licenses/>.

using FluentAutoClicker.Helpers;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentAutoClicker;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage
{
    // Pin the backdrops to prevent them being set when already set
    private static readonly MicaBackdrop MicaBackdrop = new();
    private static readonly MicaBackdrop MicaAltBackdrop = new() { Kind = MicaKind.BaseAlt };
    private static readonly DesktopAcrylicBackdrop AcrylicBackdrop = new();

    private readonly bool _isInitialized;

    public SettingsPage()
    {
        InitializeComponent();
        AppAboutSettingsExpander.Header = AppName;
        AppVersionTextBlock.Text = AppVersion;

        BackdropSelector.SelectedIndex = MainWindow.SystemBackdrop switch
        {
            MicaBackdrop { Kind: MicaKind.BaseAlt } => 1,
            DesktopAcrylicBackdrop => 2,
            _ => 0
        };

        _isInitialized = true;
    }

    public static string AppName => "AppDisplayName".GetLocalized();

    public static string AppVersion
    {
        get
        {
            PackageVersion version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }

    private static WindowEx MainWindow => App.MainWindow;

    private static int ThemeSelectedIndex
    {
        get => ((FrameworkElement)MainWindow.Content).RequestedTheme switch
        {
            ElementTheme.Light => 1,
            ElementTheme.Dark => 2,
            _ => 0
        };
        set => ((FrameworkElement)MainWindow.Content).RequestedTheme = value switch
        {
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default
        };
    }

    private void Backdrop_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isInitialized)
        {
            return;
        }

        switch (((ComboBox)sender).SelectedIndex)
        {
            case 1: MainWindow.SystemBackdrop = MicaAltBackdrop; break;
            case 2: MainWindow.SystemBackdrop = AcrylicBackdrop; break;
            default: MainWindow.SystemBackdrop = MicaBackdrop; break;
        }
    }
}