using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Fluent_Auto_Clicker.Contracts.Services;
using Fluent_Auto_Clicker.Helpers;
using Fluent_Auto_Clicker.Views;

using Microsoft.UI.Xaml.Navigation;

namespace Fluent_Auto_Clicker.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private bool _isAutoclickerRunning;
    public bool IsAutoclickerRunning
    {
        get => _isAutoclickerRunning;
        set => SetProperty(ref _isAutoclickerRunning, value);
    }

    public MainViewModel()
    {
        StartClickerCommand = new RelayCommand(StartClicker);
        StopClickerCommand = new RelayCommand(StopClicker);
    }

    public RelayCommand StartClickerCommand
    {
        get;
    }
    public RelayCommand StopClickerCommand
    {
        get;
    }

    private void StartClicker()
    {
        if (!IsAutoclickerRunning)
        {
            IsAutoclickerRunning = true;
            AutoClickerHelper.StartAutoClicker();
        }
    }

    private void StopClicker()
    {
        if (IsAutoclickerRunning)
        {
            IsAutoclickerRunning = false;
            AutoClickerHelper.StopAutoClicker();
        }
    }
}
