// Copyright (C) 2025 Ryan Luu
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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluentAutoClicker.Controls;

[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
public partial class IsEnabledTextBlock : Control
{
    public IsEnabledTextBlock()
    {
        DefaultStyleKey = typeof(IsEnabledTextBlock);
    }

    protected override void OnApplyTemplate()
    {
        IsEnabledChanged -= IsEnabledTextBlock_IsEnabledChanged;
        SetEnabledState();
        IsEnabledChanged += IsEnabledTextBlock_IsEnabledChanged;
        base.OnApplyTemplate();
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
       "Text",
       typeof(string),
       typeof(IsEnabledTextBlock),
       null);

    [Localizable(true)]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void IsEnabledTextBlock_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SetEnabledState();
    }

    private void SetEnabledState()
    {
        _ = VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
    }
}