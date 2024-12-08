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

using Microsoft.Windows.ApplicationModel.Resources;

namespace FluentAutoClicker.Helpers;

/// <summary>
/// Helper for getting localized strings from resources.
/// </summary>
public static class ResourceExtensions
{
    private static readonly ResourceLoader ResourceLoader = new();

    /// <summary>
    /// Gets the localized string for the resource key.
    /// </summary>
    /// <param name="resourceKey">The resource key for the returned localized string.</param>
    /// <returns>The localized string for the specified resource key.</returns>
    public static string GetLocalized(this string resourceKey)
    {
        return ResourceLoader.GetString(resourceKey);
    }
}