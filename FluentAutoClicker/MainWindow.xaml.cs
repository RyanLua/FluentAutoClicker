using FluentAutoClicker.Helpers;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentAutoClicker;

/// <summary>
///     An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        Title = "AppDisplayName".GetLocalized();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.SetIcon("Assets/WindowIcon.ico");

        AppTitleBar.Title = "AppDisplayName".GetLocalized();

        MainFrame.Navigate(typeof(MainPage));
        MainFrame.Navigated += OnNavigated;

        AppTitleBar.IsBackButtonVisible = false;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (e.SourcePageType == typeof(MainPage))
        {
            AppTitleBar.IsBackButtonVisible = false;
        }
        else
        {
            AppTitleBar.IsBackButtonVisible = true;
        }
    }

    private void AppTitleBar_BackRequested(Microsoft.UI.Xaml.Controls.TitleBar sender, object args)
    {
        if (MainFrame.CanGoBack)
        {
            MainFrame.GoBack();
        }
    }
}