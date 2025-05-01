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
using Microsoft.Windows.BadgeNotifications;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using WinRT.Interop;
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
    /// Initializes a new instance of the <see cref="MainPage" /> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
        Loaded += MainPage_Loaded;

        // Set tooltip
        ToolTipService.SetToolTip(ToggleButtonStart, "ToggleButtonStartTooltipStart".GetLocalized());
    }

    /// <summary>
    /// Gets or sets a value indicating whether the hotkey is registered.
    /// </summary>
    private bool IsHotKeyRegistered { get; set; }

    /// <summary>
    /// Handles the Loaded event of the MainPage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Set badge notification
        SetNotificationBadge(BadgeNotificationGlyph.Paused);

        // Prevent registering multiple hotkeys
        if (IsHotKeyRegistered)
        {
            return;
        }

        // Get window handle
        MainWindow window = App.MainWindow;
        HWND hWnd = new(WindowNative.GetWindowHandle(window));

        // Set up window message monitor
        WindowMessageMonitor monitor = new(window);
        monitor.WindowMessageReceived += OnWindowMessageReceived;

        // Register hotkey
        bool success = PInvoke.RegisterHotKey(hWnd, 0x0000, HOT_KEY_MODIFIERS.MOD_NOREPEAT, 0x75); // F6
        if (success)
        {
            IsHotKeyRegistered = true;
        }
        else
        {
            ContentDialog dialog = new()
            {
                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                XamlRoot = XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Failed to register hotkey",
                Content = "Please modify the hotkey setting.",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary
            };

            _ = await dialog.ShowAsync();
        }
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
    /// Sets the enabled state of the controls.
    /// </summary>
    /// <param name="isEnabled">if set to <c>true</c> the controls are enabled; otherwise, they are disabled.</param>
    private void SetControlsEnabled(bool isEnabled)
    {
        ClickIntervalTextBlock.IsEnabled = isEnabled;
        NumberBoxHours.IsEnabled = isEnabled;
        NumberBoxMinutes.IsEnabled = isEnabled;
        NumberBoxSeconds.IsEnabled = isEnabled;
        NumberBoxMilliseconds.IsEnabled = isEnabled;
        MouseButtonTypeComboBox.IsEnabled = isEnabled;
        HotkeyTextBlock.IsEnabled = isEnabled;
        ClickRepeatCheckBox.IsEnabled = isEnabled;
        ClickOffsetCheckBox.IsEnabled = isEnabled;
        ClickOffsetAmount.IsEnabled = isEnabled && ClickOffsetCheckBox.IsChecked == true;
        ClickRepeatAmount.IsEnabled = isEnabled && ClickRepeatCheckBox.IsChecked == true;
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
        SetControlsEnabled(false);
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
        SetControlsEnabled(true);
        FontIconStart.Glyph = "\uEE4A";
        SetNotificationBadge(BadgeNotificationGlyph.Paused);
        ToolTipService.SetToolTip(ToggleButtonStart, "ToggleButtonStartTooltipStart".GetLocalized());

        // Stop auto clicker
        AutoClicker.Stop();
    }

    /// <summary>
    /// Handles the Click event of the CheckBox controls.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        if (sender.Equals(ClickRepeatCheckBox))
        {
            ClickRepeatAmount.IsEnabled = ClickRepeatCheckBox.IsChecked == true;
            AutoClicker.clickAmountEnabled = ClickRepeatCheckBox.IsChecked == true;
        }
        else if (sender.Equals(ClickOffsetCheckBox))
        {
            ClickOffsetAmount.IsEnabled = ClickOffsetCheckBox.IsChecked == true;
            AutoClicker.clickDelayOffsetEnabled = ClickOffsetCheckBox.IsChecked == true;
        }
    }

    /// <summary>
    /// Handles the Click event of the SettingsButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        _ = Frame.Navigate(typeof(SettingsPage));
    }
}
