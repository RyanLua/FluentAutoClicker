using Fluent_Auto_Clicker.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fluent_Auto_Clicker.Views;

public sealed partial class BlankPage : Page
{
    public BlankViewModel ViewModel
    {
        get;
    }

    public BlankPage()
    {
        ViewModel = App.GetService<BlankViewModel>();
        InitializeComponent();
    }
}
