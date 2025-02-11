// Copyright (C) 2025 Ryan Luu
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
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.System;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentAutoClicker;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage
{
    public SettingsPage()
    {
        InitializeComponent();
        AppAboutSettingsExpander.Header = AppName;
        AppVersionTextBlock.Text = AppVersion;
    }

    private static string AppName => "AppDisplayName".GetLocalized();

    private static string AppVersion
    {
        get
        {
            PackageVersion version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }

    private static WindowEx MainWindow => App.MainWindow;

    private int ThemeSelectedIndex
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

    private int BackdropSelectedIndex
    {
        get => MainWindow.SystemBackdrop switch
        {
            MicaBackdrop { Kind: MicaKind.BaseAlt } => 1,
            DesktopAcrylicBackdrop => 2,
            _ => 0
        };
        set => MainWindow.SystemBackdrop = value switch
        {
            1 => new MicaBackdrop() { Kind = MicaKind.BaseAlt },
            2 => new DesktopAcrylicBackdrop(),
            _ => new MicaBackdrop()
        };
    }

    private bool IsAlwaysOnTop
    {
        get => MainWindow.IsAlwaysOnTop;
        set => MainWindow.IsAlwaysOnTop = value;
    }

    private async void HyperlinkButtonFeedback_Click(object sender, RoutedEventArgs e)
    {
        string recipientEmail = "feedback@fluentautoclicker.com";
        string subject = $"{AppName} App Feedback";
        string messageBody = $"""


            ---------- Add your feedback above ----------

            .NET installation: {RuntimeInformation.FrameworkDescription}
            App version: {AppVersion}
            App architecture: {RuntimeInformation.ProcessArchitecture}
            OS version: {RuntimeInformation.OSDescription}
            OS architecture: {RuntimeInformation.OSArchitecture}
            """;

        await ComposeEmailAsync(recipientEmail, subject, messageBody);
    }

    private static async Task ComposeEmailAsync(string recipientEmail, string subject, string messageBody)
    {
        Uri uri = new($"mailto:{recipientEmail}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(messageBody)}");

        await Launcher.LaunchUriAsync(uri);
    }
}