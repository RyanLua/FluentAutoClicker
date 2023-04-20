using Fluent_Auto_Clicker.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fluent_Auto_Clicker.Views;

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
    }
}
