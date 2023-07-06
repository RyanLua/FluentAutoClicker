using System.Diagnostics;
using Fluent_Auto_Clicker.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization.NumberFormatting;

namespace Fluent_Auto_Clicker.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            StartClicker.Checked += StartClicker_Checked;
            StartClicker.Unchecked += StartClicker_Unchecked;
        }

        private void SetClicker_Interval()
        {
            var hours = Convert.ToInt32(Interval_Hours.Value);
            var minutes = Convert.ToInt32(Interval_Minutes.Value);
            var seconds = Convert.ToInt32(Interval_Seconds.Value);
            var milliseconds = Convert.ToInt32(Interval_Milliseconds.Value);

            var totalTimeInMilliseconds = ((hours * 60 + minutes) * 60 + seconds) * 1000 + milliseconds;

            AutoClickerHelper.RepeatInterval = totalTimeInMilliseconds;
        }

        private void StartClicker_Checked(object sender, RoutedEventArgs e)
        {
            AutoClickerHelper.StartAutoClicker();
            SetClicker_Interval();
        }

        private void StartClicker_Unchecked(object sender, RoutedEventArgs e)
        {
            //AutoClickerHelper.StopAutoClicker();
        }
    }
}
