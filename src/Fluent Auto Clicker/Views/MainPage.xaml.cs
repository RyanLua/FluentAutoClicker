using Fluent_Auto_Clicker.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

        private void StartClicker_Checked(object sender, RoutedEventArgs e)
        {
            AutoClickerHelper.StartAutoClicker();
        }

        private void StartClicker_Unchecked(object sender, RoutedEventArgs e)
        {
            // Implement any logic you need when the autoclicker is stopped
        }
    }
}
