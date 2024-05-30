using System.Diagnostics;
using FluentAutoClicker.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace FluentAutoClicker.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        StartClicker.Checked += StartClicker_Checked;
        StartClicker.Unchecked += StartClicker_Unchecked;
        AutoClickerHelper.AutoClickerStopped += AutoClickerHelper_AutoClickerStopped;
    }

    private void ButtonType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var mouseButtonIndex = comboBox.SelectedIndex;

        AutoClickerHelper.MouseButton = mouseButtonIndex;

        Debug.WriteLine($"Mouse Button: {AutoClickerHelper.MouseButton}");
    }

    private void SetClicker_Interval()
    {
        var hours = Convert.ToInt32(IntervalHours.Value);
        var minutes = Convert.ToInt32(IntervalMinutes.Value);
        var seconds = Convert.ToInt32(IntervalSeconds.Value);
        var milliseconds = Convert.ToInt32(IntervalMilliseconds.Value);

        var totalTimeInMilliseconds = ((hours * 60 + minutes) * 60 + seconds) * 1000 + milliseconds;

        AutoClickerHelper.ClickInterval = totalTimeInMilliseconds;

        Debug.WriteLine($"Interval Time (Milliseconds): {AutoClickerHelper.ClickInterval}");
    }

    private void IntervalNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args) =>
        SetClicker_Interval();

    private void RepeatTypeRadioButtons_SelectionChanged(object sender, RoutedEventArgs e) => SetClicker_Repeat();

    private void RepeatTypeNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        SetClicker_Repeat();
        if (RepeatTypeClicks.Value != 100)
        {
            RepeatTypeRadioButtons.SelectedIndex = 0;
        }
    }

    private void SetClicker_Repeat()
    {
        var repeatCount = Convert.ToInt32(RepeatTypeClicks.Value);
        var repeatTypeIndex = RepeatTypeRadioButtons.SelectedIndex;

        if (repeatTypeIndex == 0)
        {
            AutoClickerHelper.RepeatAmount = repeatCount;
        }
        else
        {
            AutoClickerHelper.RepeatAmount = 0;
        }

        Debug.WriteLine($"Repeat Count: {AutoClickerHelper.RepeatAmount}");
    }

    private async void StartClicker_Checked(object sender, RoutedEventArgs e)
    {
        var toggleButton = (ToggleButton)sender;
        toggleButton.IsEnabled = false; // Uncheck the button

        for (var i = 3; i > 0; i--)
        {
            toggleButton.Content = i.ToString();
            await Task.Delay(1000);
        }

        toggleButton.IsEnabled = true;
        toggleButton.Content = "Stop";
        AutoClickerHelper.StartAutoClicker();
        SetClicker_Interval();
        SetClicker_Repeat();

        Debug.WriteLine($"Auto Clicker Running: {AutoClickerHelper.IsAutoClickerRunning}");
    }

    private static void StartClicker_Unchecked(object sender, RoutedEventArgs e)
    {
        var toggleButton = (ToggleButton)sender;
        toggleButton.Content = "Start";
        AutoClickerHelper.StopAutoClicker();
        Debug.WriteLine($"Auto Clicker Stopped: {AutoClickerHelper.IsAutoClickerRunning}");
    }

    private void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        if (AutoClickerHelper.IsAutoClickerRunning)
        {
            AutoClickerHelper.StopAutoClicker();
            Debug.WriteLine($"Auto Clicker Stopped: {AutoClickerHelper.IsAutoClickerRunning}");
        }
        else
        {
            AutoClickerHelper.StartAutoClicker();
            SetClicker_Interval();
            SetClicker_Repeat();

            Debug.WriteLine($"Auto Clicker Running: {AutoClickerHelper.IsAutoClickerRunning}");
        }
    }

    private async void HotkeyButton_Click(object sender, RoutedEventArgs e)
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

        var result = await dialog.ShowAsync();
    }

    private void AutoClickerHelper_AutoClickerStopped() =>
        _ = DispatcherQueue.TryEnqueue(() =>
        {
            StartClicker.IsChecked = false;
        });
}