// Copyright (C) 2024 Ryan Luu
//
// This file is part of Fluent Auto Clicker.
//
// Fluent Auto Clicker is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Fluent Auto Clicker is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Fluent Auto Clicker. If not, see <https://www.gnu.org/licenses/>.

using FluentAutoClicker.Helpers;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentAutoClicker;

/// <summary>
///     An window that displays a page's contents.
/// </summary>
public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        // Set up window
        Title = "AppDisplayName".GetLocalized();
        AppWindow.SetIcon("Assets/WindowIcon.ico");

        // Set up window title bar
        ExtendsContentIntoTitleBar = true;
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