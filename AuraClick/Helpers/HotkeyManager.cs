// Copyright (C) 2025 Ryan Luu
//
// This file is part of Aura Click.
//
// Aura Click is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Aura Click is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Aura Click. If not, see <https://www.gnu.org/licenses/>.

using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using WinRT.Interop;

namespace AuraClick.Helpers;

internal class HotkeyManager
{
    public static bool RegisterHotkey(int hotkeyId, VirtualKeyModifiers modifiers, VirtualKey virtualKey)
    {
        // Get window handle
        MainWindow window = App.MainWindow;
        HWND hWnd = new(WindowNative.GetWindowHandle(window));

        UnregisterHotkey(hotkeyId);

        // Register hotkey
        BOOL success = PInvoke.RegisterHotKey(hWnd, hotkeyId, HOT_KEY_MODIFIERS.MOD_NOREPEAT | (HOT_KEY_MODIFIERS)modifiers, (uint)virtualKey);

        return success;
    }

    public static bool UnregisterHotkey(int hotkeyId)
    {
        // Get window handle
        MainWindow window = App.MainWindow;
        HWND hWnd = new(WindowNative.GetWindowHandle(window));

        // Unregister hotkey
        BOOL success = PInvoke.UnregisterHotKey(hWnd, hotkeyId);

        return success;
    }
}
