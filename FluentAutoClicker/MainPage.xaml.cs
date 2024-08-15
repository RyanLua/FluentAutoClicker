using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FluentAutoClicker.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Automation.Peers;

namespace FluentAutoClicker
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var hook = new WindowMessageHook(App.Window);
            Unloaded += (s, e) => hook.Dispose(); // unhook on close
            hook.Message += (s, e) =>
            {
                const int WM_HOTKEY = 0x312;
                if (e.Message == WM_HOTKEY)
                {
                    // click on the button using UI Automation
                    var pattern = (ToggleButtonAutomationPeer)FrameworkElementAutomationPeer.FromElement(StartToggleButton).GetPattern(PatternInterface.Toggle);
                    pattern.Toggle();
                }
            };

            // register CTRL + B as a global hotkey
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Window);
            var id = 1; // some arbitrary hotkey identifier
            if (!RegisterHotKey(hwnd, id, MOD.MOD_CONTROL, VirtualKey.F6))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Unloaded += (s, e) => UnregisterHotKey(hwnd, id); // unregister hotkey on window close
        }

    private void SetClicker_Interval()
    {
        if (!Int32.TryParse(NumberBoxHours.Value.ToString(CultureInfo.InvariantCulture), out var hours))
        {
            hours = 0;
            NumberBoxHours.Value = hours;
        }

        if (!Int32.TryParse(NumberBoxMinutes.Value.ToString(CultureInfo.InvariantCulture), out var minutes))
        {
            minutes = 0;
            NumberBoxMinutes.Value = minutes;
        }

        if (!Int32.TryParse(NumberBoxSeconds.Value.ToString(CultureInfo.InvariantCulture), out var seconds))
        {
            seconds = 0;
            NumberBoxSeconds.Value = seconds;
        }

        if (!Int32.TryParse(NumberBoxMilliseconds.Value.ToString(CultureInfo.InvariantCulture), out var milliseconds))
        {
            milliseconds = 100;
            NumberBoxMilliseconds.Value = milliseconds;
        }

        var totalTimeInMilliseconds = ((hours * 60 + minutes) * 60 + seconds) * 1000 + milliseconds;

        if (totalTimeInMilliseconds == 0)
        {
            totalTimeInMilliseconds = 1;
            NumberBoxMilliseconds.Value = 1;
        }

        AutoClicker.ClickInterval = totalTimeInMilliseconds;
    }

    private void MouseButtonType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        AutoClicker.MouseButton = MouseButtonTypeComboBox.SelectedIndex;
    }

    private async void HotkeyButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Set hotkey",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Reset",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = "Press any key to bind it as a hotkey."
        };

        await dialog.ShowAsync();
    }

    private void ClickRepeatType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ClickRepeatType.SelectedIndex == 0)
        {
            ClickRepeatAmount.IsEnabled = true;
            AutoClicker.RepeatAmount = (int)ClickRepeatAmount.Value;
        }
        else
        {
            ClickRepeatAmount.IsEnabled = false;
            AutoClicker.RepeatAmount = 0;
        }
    }

    private async void StartToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        StartToggleButton.IsEnabled = false;

        // 3-second countdown
        for (var i = 3; i > 0; i--)
        {
            StartToggleButton.Content = i.ToString();
            await Task.Delay(1000);
        }

        StartToggleButton.IsEnabled = true;
        StartToggleButton.Content = "Stop";
        AutoClicker.StartAutoClicker();
        SetClicker_Interval();
    }

    private void ClickRepeatAmount_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        AutoClicker.RepeatAmount = (int)ClickRepeatAmount.Value;
    }

    private void StartToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        StartToggleButton.Content = "Start";
        AutoClicker.StopAutoClicker();
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
    }
}
