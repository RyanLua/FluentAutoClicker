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

using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace FluentAutoClicker.Helpers;

/// <summary>
/// Helper for creating threads to synthesize mouse input.
/// </summary>
public static class AutoClicker
{
    private static Thread? _autoClickerThread;
    private static bool _isAutoClickerRunning;

    /// <summary>
    /// Starts the auto clicker thread.
    /// </summary>
    /// <param name="millisecondsDelay">The number of milliseconds to wait before clicks.</param>
    /// <param name="clickAmount">The number of clicks before stopping the auto clicker thread.</param>
    /// <param name="mouseButtonType">The mouse button used to click.</param>
    /// <param name="clickDelayOffset">Milliseconds to add randomly to delay between clicks.</param>
    public static void Start(int millisecondsDelay = 100, int clickAmount = 0,
        int mouseButtonType = 0, int clickDelayOffset = 0)
    {
        // TODO: Move the parameters to another function to be able to change parameters while the thread is running.
        _isAutoClickerRunning = true;
        _autoClickerThread = new Thread(() =>
            AutoClickerThread(millisecondsDelay, clickAmount, mouseButtonType, clickDelayOffset));
        _autoClickerThread.Start();
    }

    /// <summary>
    /// Stops the auto clicker thread.
    /// </summary>
    public static void Stop()
    {
        _isAutoClickerRunning = false;
        _autoClickerThread?.Join();
    }

    private static async void AutoClickerThread(int clickInterval, int repeatAmount, int mouseButton,
        int clickOffset)
    {
        int clickCount = 0;

        while (_isAutoClickerRunning)
        {
            // Stop if we click more than repeat amount
            if (clickCount >= repeatAmount && repeatAmount != 0)
            {
                Stop();
                break;
            }

            // Click mouse and increment click count
            ClickMouse(mouseButton);
            clickCount++;

            // Delay before next click
            int randomClickOffset = new Random().Next(0, clickOffset);
            await Task.Delay(clickInterval + randomClickOffset);
        }
    }

    /// <summary>
    /// Clicks the mouse button.
    /// </summary>
    /// <param name="button">The mouse button to click.</param>
    private static void ClickMouse(int button)
    {
        switch (button)
        {
            // Left mouse button
            case 0:
                SendMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN);
                SendMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP);
                break;
            // Middle mouse button
            case 1:
                SendMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN);
                SendMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP);
                break;
            // Right mouse button
            case 2:
                SendMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN);
                SendMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP);
                break;
        }
    }

    /// <summary>
    /// Sends a mouse input event.
    /// </summary>
    /// <param name="dwFlags">The mouse event flags that specify the type of mouse event.</param>
    private static void SendMouseInput(MOUSE_EVENT_FLAGS dwFlags)
    {
        INPUT[] inputs =
        [
            new()
            {
                type = INPUT_TYPE.INPUT_MOUSE,
                Anonymous = new INPUT._Anonymous_e__Union { mi = new MOUSEINPUT { dwFlags = dwFlags } }
            }
        ];

        _ = PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
    }
}