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
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
        WindowMessageHook hook = new(App.Window);
        Unloaded += (s, e) => hook.Dispose(); // unhook on close
        hook.Message += (s, e) =>
        {
            const int WM_HOTKEY = 0x312;
            if (e.Message == WM_HOTKEY)
            {
                // click on the button using UI Automation
                ToggleButtonAutomationPeer pattern = (ToggleButtonAutomationPeer)FrameworkElementAutomationPeer.FromElement(StartToggleButton).GetPattern(PatternInterface.Toggle);
                pattern.Toggle();
            }
        };

        // register CTRL + B as a global hotkey
        nint hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Window);
        int id = 1; // some arbitrary hotkey identifier
        if (!RegisterHotKey(hwnd, id, MOD.MOD_NOREPEAT, VirtualKey.F6))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        Unloaded += (s, e) => UnregisterHotKey(hwnd, id); // unregister hotkey on window close
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
        //HotkeyButton.IsEnabled = isEnabled; 

        if (ClickOffsetCheckBox.IsChecked == true)
        {
            ClickOffsetAmount.IsEnabled = isEnabled;
        }

        if (ClickRepeatCheckBox.IsChecked == true)
        {
            ClickRepeatAmount.IsEnabled = isEnabled;
        }

        // TODO: Change this to use a custom control. See https://github.com/RyanLua/FluentAutoClicker/issues/42
        if (!isEnabled)
        {
            ClickIntervalTextBlock.Foreground = Application.Current.Resources["SystemControlForegroundBaseMediumLowBrush"] as Brush;
            HotkeyTextBlock.Foreground = Application.Current.Resources["SystemControlForegroundBaseMediumLowBrush"] as Brush;
        }
        else
        {
            ClickIntervalTextBlock.Foreground = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as Brush;
            HotkeyTextBlock.Foreground = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as Brush;
        }
    }

    private int GetIntervalMilliseconds()
    {
        if (!int.TryParse(NumberBoxHours.Value.ToString(CultureInfo.InvariantCulture), out int hours))
        {
            hours = 0;
            NumberBoxHours.Value = hours;
        }

        if (!int.TryParse(NumberBoxMinutes.Value.ToString(CultureInfo.InvariantCulture), out int minutes))
        {
            minutes = 0;
            NumberBoxMinutes.Value = minutes;
        }

        if (!int.TryParse(NumberBoxSeconds.Value.ToString(CultureInfo.InvariantCulture), out int seconds))
        {
            seconds = 0;
            NumberBoxSeconds.Value = seconds;
        }

        if (!int.TryParse(NumberBoxMilliseconds.Value.ToString(CultureInfo.InvariantCulture), out int milliseconds))
        {
            milliseconds = 100;
            NumberBoxMilliseconds.Value = milliseconds;
        }

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

    // interop code for Windows API hotkey functions
    [DllImport("user32", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, MOD fsModifiers, VirtualKey vk);

    [DllImport("user32", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

    [Flags]
    private enum MOD
    {
        MOD_ALT = 0x1,
        MOD_CONTROL = 0x2,
        MOD_SHIFT = 0x4,
        MOD_WIN = 0x8,
        MOD_NOREPEAT = 0x4000,
    }
    private void ClickOffsetCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ClickOffsetAmount.IsEnabled = false;
    }

    private void ClickOffsetCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ClickOffsetAmount.IsEnabled = true;
    }
}