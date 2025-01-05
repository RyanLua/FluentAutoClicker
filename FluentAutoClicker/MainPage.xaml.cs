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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using Windows.System;

namespace FluentAutoClicker;

/// <summary>
/// The main page containing all controls displayed on the main window.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        MainWindow window = App.MainWindow;
        nint hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        WindowMessageHook hook = new(window);
        Unloaded += (s, e) => hook.Dispose();

        hook.Message += (s, e) =>
        {
            const int wmHotkey = 0x312;
            if (e.Message == wmHotkey)
            {
                // Toggle the StartToggleButton when the hotkey is pressed
                ToggleButtonAutomationPeer pattern = (ToggleButtonAutomationPeer)FrameworkElementAutomationPeer.FromElement(StartToggleButton).GetPattern(PatternInterface.Toggle);
                pattern.Toggle();
            }
        };

        int id = 1;

        // Register the F6 key
        if (!RegisterHotKey(hWnd, id, VirtualKeyModifiers.None, VirtualKey.F6))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        Unloaded += (s, e) => UnregisterHotKey(hWnd, id);
    }

    private void SetControlsEnabled(bool isEnabled)
    {
        NumberBoxHours.IsEnabled = isEnabled;
        NumberBoxMinutes.IsEnabled = isEnabled;
        NumberBoxSeconds.IsEnabled = isEnabled;
        NumberBoxMilliseconds.IsEnabled = isEnabled;
        MouseButtonTypeComboBox.IsEnabled = isEnabled;
        ClickRepeatCheckBox.IsEnabled = isEnabled;
        ClickOffsetCheckBox.IsEnabled = isEnabled;

        ClickOffsetAmount.IsEnabled = ClickOffsetCheckBox.IsChecked == true && isEnabled;
        ClickRepeatAmount.IsEnabled = ClickRepeatCheckBox.IsChecked == true && isEnabled;

        // TODO: Change this to use a custom control. See https://github.com/RyanLua/FluentAutoClicker/issues/42
        string brushKey = isEnabled ? "SystemControlForegroundBaseHighBrush" : "SystemControlForegroundBaseMediumLowBrush";
        ClickIntervalTextBlock.Foreground = Application.Current.Resources[brushKey] as Brush;
        HotkeyTextBlock.Foreground = Application.Current.Resources[brushKey] as Brush;
    }

    private int GetNumberBoxValue(NumberBox numberBox, int defaultValue)
    {
        if (!int.TryParse(numberBox.Value.ToString(CultureInfo.InvariantCulture), out int value))
        {
            value = defaultValue;
            numberBox.Value = value;
        }
        return value;
    }

    private int GetIntervalMilliseconds()
    {
        int hours = GetNumberBoxValue(NumberBoxHours, 0);
        int minutes = GetNumberBoxValue(NumberBoxMinutes, 0);
        int seconds = GetNumberBoxValue(NumberBoxSeconds, 0);
        int milliseconds = GetNumberBoxValue(NumberBoxMilliseconds, 100);

        int totalTimeInMilliseconds = (((((hours * 60) + minutes) * 60) + seconds) * 1000) + milliseconds;

        if (totalTimeInMilliseconds == 0)
        {
            totalTimeInMilliseconds = 1;
            NumberBoxMilliseconds.Value = 1;
        }

        return totalTimeInMilliseconds;
    }

    private async void StartToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        StartToggleButton.IsEnabled = false;
        SetControlsEnabled(false);

        // 3-second countdown
        for (int i = 3; i > 0; i--)
        {
            StartToggleButton.Content = i.ToString();
            await Task.Delay(1000);
        }

        StartToggleButton.IsEnabled = true;
        StartToggleButton.Content = "Stop";

        int clickInterval = GetIntervalMilliseconds();
        int repeatAmount = ClickRepeatCheckBox.IsEnabled == true ? Convert.ToInt32(ClickRepeatAmount.Value) : 0;
        int mouseButton = MouseButtonTypeComboBox.SelectedIndex;
        int clickOffset = ClickOffsetCheckBox.IsChecked == true ? Convert.ToInt32(ClickOffsetAmount.Value) : 0;
        AutoClicker.StartAutoClicker(clickInterval, repeatAmount, mouseButton, clickOffset);
    }

    private void StartToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        StartToggleButton.Content = "Start";
        AutoClicker.StopAutoClicker();
        SetControlsEnabled(true);
    }

    private void ClickRepeatCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ClickRepeatAmount.IsEnabled = false;
    }

    private void ClickRepeatCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ClickRepeatAmount.IsEnabled = true;
    }

    /// <summary>
    /// Define a system-wide hot key.
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the window that will receive WM_HOTKEY messages generated by the
    /// hot key. If this parameter is NULL, WM_HOTKEY messages are posted to the 
    /// message queue of the calling thread and must be processed in the message loop.
    /// </param>
    /// <param name="id">
    /// The identifier of the hot key. If the hWnd parameter is NULL, then the hot 
    /// key is associated with the current thread rather than with a particular 
    /// window. 
    /// </param>
    /// <param name="fsModifiers">
    /// The keys that must be pressed in combination with the key specified by the 
    /// uVirtKey parameter in order to generate the WM_HOTKEY message.
    /// </param>
    /// <param name="vk">The virtual-key code of the hot key.</param>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RegisterHotKey(nint hWnd, int id, VirtualKeyModifiers fsModifiers, VirtualKey vk);

    /// <summary>
    /// Frees a hot key previously registered by the calling thread. 
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the window associated with the hot key to be freed. This parameter
    /// should be NULL if the hot key is not associated with a window.
    /// </param>
    /// <param name="id">
    /// The identifier of the hot key to be freed. 
    /// </param>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnregisterHotKey(nint hWnd, int id);

    private void ClickOffsetCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ClickOffsetAmount.IsEnabled = false;
    }

    private void ClickOffsetCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ClickOffsetAmount.IsEnabled = true;
    }
}