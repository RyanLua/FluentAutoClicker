using Fluent_Auto_Clicker.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fluent_Auto_Clicker.Views;

public sealed partial class SpeedtestPage : Page
{
    public SpeedtestViewModel ViewModel
    {
        get;
    }

    public SpeedtestPage()
    {
        ViewModel = App.GetService<SpeedtestViewModel>();
        InitializeComponent();
    }
}
