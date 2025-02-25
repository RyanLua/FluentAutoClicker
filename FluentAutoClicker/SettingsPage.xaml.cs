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
using Microsoft.UI.Xaml.Media;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using WinUIEx;
using SystemBackdrop = Microsoft.UI.Xaml.Media.SystemBackdrop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentAutoClicker;

/// <summary>
/// Represents the settings page of the Fluent Auto Clicker application.
/// </summary>
public sealed partial class SettingsPage
{
    /// <summary>
    /// Application settings container in the local app data store.
    /// </summary>
    private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage" /> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();

        // Initialize saved settings
        ThemeSelectedIndex = ThemeSelectedIndex;
        BackdropSelectedIndex = BackdropSelectedIndex;
        IsAlwaysOnTop = IsAlwaysOnTop;

        // Initialize app name and version
        AppAboutSettingsExpander.Header = AppName;
        AppVersionTextBlock.Text = AppVersion;
    }

    /// <summary>
    /// Gets or sets the if the playing notification badge is enabled.
    /// </summary>
    public bool NotificationBadgePlaying
    {
        get => (bool)(localSettings.Values[nameof(NotificationBadgePlaying)] ?? true);
        set => localSettings.Values[nameof(NotificationBadgePlaying)] = value;
    }

    /// <summary>
    /// Gets or sets the if the paused notification badge is enabled.
    /// </summary>
    public bool NotificationBadgePaused
    {
        get => (bool)(localSettings.Values[nameof(NotificationBadgePaused)] ?? true);
        set => localSettings.Values[nameof(NotificationBadgePaused)] = value;
    }

    /// <summary>
    /// Gets the localized application name.
    /// </summary>
    private static string AppName => "AppDisplayName".GetLocalized();

    /// <summary>
    /// Gets the application version.
    /// </summary>
    private static string AppVersion
    {
        get
        {
            PackageVersion version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }

    /// <summary>
    /// Gets the main application window.
    /// </summary>
    private static WindowEx MainWindow => App.MainWindow;

    /// <summary>
    /// Gets or sets the selected theme index.
    /// </summary>
    private int ThemeSelectedIndex
    {
        get => (int)(localSettings.Values[nameof(ThemeSelectedIndex)] ?? 0);
        set
        {
            localSettings.Values[nameof(ThemeSelectedIndex)] = value;
            ((FrameworkElement)MainWindow.Content).RequestedTheme = value switch
            {
                1 => ElementTheme.Light,
                2 => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
        }
    }

    /// <summary>
    /// Gets or sets the selected backdrop index.
    /// </summary>
    private int BackdropSelectedIndex
    {
        get => (int)(localSettings.Values[nameof(BackdropSelectedIndex)] ?? 0);
        set
        {
            localSettings.Values[nameof(BackdropSelectedIndex)] = value;

            // HACK: Prevent changing the backdrop so it doesn't flash between pages
            SystemBackdrop currentBackdrop = MainWindow.SystemBackdrop;
            bool needsChange = value switch
            {
                1 => currentBackdrop is not MicaBackdrop { Kind: MicaKind.BaseAlt },
                2 => currentBackdrop is not DesktopAcrylicBackdrop,
                _ => currentBackdrop is not MicaBackdrop ||
                     (currentBackdrop is MicaBackdrop mica && mica.Kind != MicaKind.Base)
            };

            if (needsChange)
            {
                MainWindow.SystemBackdrop = value switch
                {
                    1 => new MicaBackdrop { Kind = MicaKind.BaseAlt },
                    2 => new DesktopAcrylicBackdrop(),
                    _ => new MicaBackdrop()
                };
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application window is always on top.
    /// </summary>
    private bool IsAlwaysOnTop
    {
        get => (bool)(localSettings.Values[nameof(IsAlwaysOnTop)] ?? true);
        set
        {
            localSettings.Values[nameof(IsAlwaysOnTop)] = value;
            MainWindow.IsAlwaysOnTop = value;
        }
    }

    /// <summary>
    /// Handles the click event of the feedback hyperlink button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void HyperlinkButtonFeedback_Click(object sender, RoutedEventArgs e)
    {
        string recipientEmail = "feedback@fluentautoclicker.com";
        string subject = $"{AppName} App Feedback";
        string messageBody = $"""


                              ----- Add your feedback above -----

                              .NET installation: {RuntimeInformation.FrameworkDescription}
                              App version: {AppVersion}
                              App architecture: {RuntimeInformation.ProcessArchitecture}
                              OS version: {RuntimeInformation.OSDescription}
                              OS architecture: {RuntimeInformation.OSArchitecture}
                              """;

        await ComposeEmailAsync(recipientEmail, subject, messageBody);
    }

    /// <summary>
    /// Composes and launches an email with the specified recipient, subject, and message body.
    /// </summary>
    /// <param name="recipientEmail">The recipient email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="messageBody">The email message body.</param>
    private static async Task ComposeEmailAsync(string recipientEmail, string subject, string messageBody)
    {
        Uri uri = new(
            $"mailto:{recipientEmail}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(messageBody)}");

        _ = await Launcher.LaunchUriAsync(uri);
    }
}