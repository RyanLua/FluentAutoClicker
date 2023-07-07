using System.Diagnostics;
using Fluent_Auto_Clicker.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization.NumberFormatting;

namespace Fluent_Auto_Clicker.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        StartClicker.Checked += StartClicker_Checked;
        StartClicker.Unchecked += StartClicker_Unchecked;
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
        var hours = Convert.ToInt32(Interval_Hours.Value);
        var minutes = Convert.ToInt32(Interval_Minutes.Value);
        var seconds = Convert.ToInt32(Interval_Seconds.Value);
        var milliseconds = Convert.ToInt32(Interval_Milliseconds.Value);

        var totalTimeInMilliseconds = ((hours * 60 + minutes) * 60 + seconds) * 1000 + milliseconds;

        AutoClickerHelper.RepeatInterval = totalTimeInMilliseconds;

        Debug.WriteLine($"Interval Time (Milliseconds): {AutoClickerHelper.RepeatInterval}");
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
        if (RepeatType_Clicks.Value != 100)
        {
            RepeatTypeRadioButtons.SelectedIndex = 0;
        }
    }

    private void SetClicker_Repeat()
    {
        var repeatCount = Convert.ToInt32(RepeatType_Clicks.Value);
        var repeatTypeIndex = RepeatTypeRadioButtons.SelectedIndex;

        if (repeatTypeIndex == 0)
        {
            AutoClickerHelper.RepeatCount = repeatCount;
        }
        else
        {
            AutoClickerHelper.RepeatCount = 0;
        }

        Debug.WriteLine($"Repeat Count: {AutoClickerHelper.RepeatCount}");
    }


    private void StartClicker_Checked(object sender, RoutedEventArgs e)
    {
        AutoClickerHelper.StartAutoClicker();
        SetClicker_Interval();
        SetClicker_Repeat();

        Debug.WriteLine($"Auto Clicker Running: {AutoClickerHelper.IsAutoClickerRunning}");
    }

    private void StartClicker_Unchecked(object sender, RoutedEventArgs e)
    {
        //AutoClickerHelper.StopAutoClicker();
        Debug.WriteLine($"Auto Clicker Stopped: {AutoClickerHelper.IsAutoClickerRunning}");
    }
}
