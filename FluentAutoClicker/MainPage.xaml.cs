// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAutoClicker.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FluentAutoClicker;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void SetControlsEnabled(bool isEnabled)
    {
        NumberBoxHours.IsEnabled = isEnabled;
        NumberBoxMinutes.IsEnabled = isEnabled;
        NumberBoxSeconds.IsEnabled = isEnabled;
        NumberBoxMilliseconds.IsEnabled = isEnabled;
        MouseButtonTypeComboBox.IsEnabled = isEnabled;
        ClickRepeatType.IsEnabled = isEnabled;
        ClickRepeatAmount.IsEnabled = isEnabled;
        //HotkeyButton.IsEnabled = isEnabled; 

        // Gray out text if disabled
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
        SetControlsEnabled(false);

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
        SetControlsEnabled(true);
    }
}