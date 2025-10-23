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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.BadgeNotifications;
using System.Diagnostics;
using WinUIEx.Messaging;

namespace FluentAutoClicker;

/// <summary>
/// The main page containing all controls displayed on the main window.
/// </summary>
public sealed partial class MainPage
{
    /// <summary>
    /// The settings page instance.
    /// </summary>
    private static readonly SettingsPage settingsPage = new();

    /// <summary>
    /// Determines whether a control should be enabled based on a checkbox state and a running-state toggle.
    /// </summary>
    /// <param name="isChecked">The state of the enabling CheckBox.</param>
    /// <param name="isRunning">The state of the start/running toggle.</param>
    /// <returns> <c>true</c> to enable the control when the checkbox is checked and the running toggle is off; otherwise, <c>false</c>.</returns>
    private bool IsControlEnabled(bool? isChecked, bool? isRunning)
    {
        return (isChecked ?? false) && !(isRunning ?? false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage" /> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
        Loaded += MainPage_Loaded;

        // Set shortcut keys
        ToggleShortcut.Keys = ["F6"];

        // Set tooltip
        ToolTipService.SetToolTip(ToggleButtonStart, "ToggleButtonStartTooltipStart".GetLocalized());
    }

    /// <summary>
    /// Handles the Loaded event of the MainPage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Set badge notification
        SetNotificationBadge(BadgeNotificationGlyph.Paused);

        // Get window handle
        MainWindow window = App.MainWindow;

        // Set up window message monitor
        WindowMessageMonitor monitor = new(window);
        monitor.WindowMessageReceived += OnWindowMessageReceived;
    }

    /// <summary>
    /// Handles the WindowMessageReceived event of the WindowMessageMonitor control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnWindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        if (e.Message.MessageId == 0x0312) // WM_HOTKEY event
        {
            if (ToggleButtonStart.IsEnabled)
            {
                ToggleButtonStart.IsChecked = !ToggleButtonStart.IsChecked;
            }
        }
    }

    /// <summary>
    /// Sets the notification badge.
    /// </summary>
    /// <param name="glyph">The badge notification glyph.</param>
    private static void SetNotificationBadge(BadgeNotificationGlyph glyph)
    {
        if (glyph == BadgeNotificationGlyph.Paused && settingsPage.NotificationBadgePaused)
        {
            BadgeNotificationManager.Current.SetBadgeAsGlyph(glyph);
        }
        else if (glyph == BadgeNotificationGlyph.Playing && settingsPage.NotificationBadgePlaying)
        {
            BadgeNotificationManager.Current.SetBadgeAsGlyph(glyph);
        }
        else
        {
            BadgeNotificationManager.Current.ClearBadge();
        }
    }

    /// <summary>
    /// Handles the Checked event of the ToggleButtonStart control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void ToggleButtonStart_OnChecked(object sender, RoutedEventArgs e)
    {
        // Update controls
        ToggleButtonStart.IsEnabled = false;
        await Task.Delay(1000);
        FontIconStart.Glyph = "\uEDB4";
        SetNotificationBadge(BadgeNotificationGlyph.Playing);
        ToolTipService.SetToolTip(ToggleButtonStart, "ToggleButtonStartTooltipStop".GetLocalized());

        // Start auto clicker
        AutoClicker.Start();
        ToggleButtonStart.IsEnabled = true;
    }

    /// <summary>
    /// Handles the Unchecked event of the ToggleButtonStart control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void ToggleButtonStart_OnUnchecked(object sender, RoutedEventArgs e)
    {
        // Update controls
        FontIconStart.Glyph = "\uEE4A";
        SetNotificationBadge(BadgeNotificationGlyph.Paused);
        ToolTipService.SetToolTip(ToggleButtonStart, "ToggleButtonStartTooltipStart".GetLocalized());

        // Stop auto clicker
        AutoClicker.Stop();
    }

    /// <summary>
    /// Handles the Click event of the SettingsButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        _ = Frame.Navigate(typeof(SettingsPage), null, new SuppressNavigationTransitionInfo());
    }

    private void ToggleShortcut_PrimaryButtonClick(object sender, ContentDialogButtonClickEventArgs e)
    {
        ToggleShortcut.UpdatePreviewKeys();
        ToggleShortcut.CloseContentDialog();

        Debug.WriteLine("New hotkey saved: " + string.Join(" + ", ToggleShortcut.Keys));
    }
}
