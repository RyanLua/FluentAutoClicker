using Microsoft.UI.Xaml;

namespace Fluent_Auto_Clicker.Contracts.Services;

public interface IAlwaysOnTopService
{
    bool AlwaysOnTop
    {
        get;
    }

    Task InitializeAsync();

    Task SetAlwaysOnTopAsync(bool AlwaysOnTop);

    Task SetRequestedAlwaysOnTopAsync();
}
