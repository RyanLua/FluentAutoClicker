using FluentAutoClicker.Helpers;
using FluentAutoClicker.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FluentAutoClicker.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        StartClicker.Checked += StartClicker_Checked;
        StartClicker.Unchecked += StartClicker_Unchecked;
    }

    private void ButtonType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var mouseButtonIndex = comboBox.SelectedIndex;

        AutoClickerHelper.MouseButton = mouseButtonIndex;
    }

    private void SetClicker_Interval()
    {
        var hours = Convert.ToInt32(IntervalHours.Value);
        var minutes = Convert.ToInt32(IntervalMinutes.Value);
        var seconds = Convert.ToInt32(IntervalSeconds.Value);
        var milliseconds = Convert.ToInt32(IntervalMilliseconds.Value);

        var totalTimeInMilliseconds = ((hours * 60 + minutes) * 60 + seconds) * 1000 + milliseconds;

        AutoClickerHelper.ClickInterval = totalTimeInMilliseconds;
    }

    private void IntervalNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        SetClicker_Interval();
    }

    private void RepeatTypeRadioButtons_SelectionChanged(object sender, RoutedEventArgs e)
    {
        SetClicker_Repeat();
    }

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
    }


    private void StartClicker_Checked(object sender, RoutedEventArgs e)
    {
        Thread.Sleep(1000);
        AutoClickerHelper.StartAutoClicker();
        SetClicker_Interval();
        SetClicker_Repeat();
    }

    private void StartClicker_Unchecked(object sender, RoutedEventArgs e)
    {
        AutoClickerHelper.StopAutoClicker();
    }

    private void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        if (AutoClickerHelper.IsAutoClickerRunning)
        {
            AutoClickerHelper.StopAutoClicker();
        }
        else
        {
            AutoClickerHelper.StartAutoClicker();
            SetClicker_Interval();
            SetClicker_Repeat();
        }
    }

    private async void HotkeyButton_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Set hotkey";
        dialog.PrimaryButtonText = "Save";
        dialog.SecondaryButtonText = "Reset";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = "Press any key to bind it as a hotkey.";

        var result = await dialog.ShowAsync();
    }
}
