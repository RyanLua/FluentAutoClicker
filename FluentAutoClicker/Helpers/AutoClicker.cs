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

using System.Runtime.InteropServices;

namespace FluentAutoClicker.Helpers;

/// <summary>
/// Helper for creating threads to synthesize mouse input.
/// </summary>
public static class AutoClicker
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

    private static Thread _autoClickerThread;
    private static bool IsAutoClickerRunning;

    /// <summary>
    /// Starts the auto clicker thread.
    /// </summary>
    /// <param name="millisecondsDelay">The number of milliseconds to wait before clicks.</param>
    /// <param name="clickAmount">The number of clicks before stopping the auto clicker thread.</param>
    /// <param name="mouseButtonType">The mouse button used to click.</param>
    /// <param name="clickDelayOffset">The amount of time in milliseconds to add randomly to the millisecond delay between clicks.</param>
    public static void StartAutoClicker(int millisecondsDelay, int clickAmount, int mouseButtonType, int clickDelayOffset)
    {
        // TODO: Evaluate whether a thread is necessary for this.
        IsAutoClickerRunning = true;
        _autoClickerThread = new Thread(() => AutoClickerThread(millisecondsDelay, clickAmount, mouseButtonType, clickDelayOffset));
        _autoClickerThread.Start();
    }

    /// <summary>
    /// Stops the auto clicker thread.
    /// </summary>
    public static void StopAutoClicker()
    {
        IsAutoClickerRunning = false;
        // HACK: Incorrectly stops the thread, but it works for now.
        _autoClickerThread?.Join();
    }

    private static async void AutoClickerThread(int ClickInterval, int RepeatAmount, int MouseButton, int ClickOffset)
    {
        int clickCount = 0;
        Random random = new();
        while (IsAutoClickerRunning)
        {
            if (clickCount >= RepeatAmount && RepeatAmount != 0)
            {
                StopAutoClicker();
                break;
            }

            // TODO: Move this to a enum instead of a number
            switch (MouseButton)
            {
                case 0:
                    MouseEvent(0, 0, (uint)MouseEventF.LeftDown, 0, 0, IntPtr.Zero);
                    MouseEvent(0, 0, (uint)MouseEventF.LeftUp, 0, 0, IntPtr.Zero);
                    break;
                case 1:
                    MouseEvent(0, 0, (uint)MouseEventF.MiddleDown, 0, 0, IntPtr.Zero);
                    MouseEvent(0, 0, (uint)MouseEventF.MiddleUp, 0, 0, IntPtr.Zero);
                    break;
                case 2:
                    MouseEvent(0, 0, (uint)MouseEventF.RightDown, 0, 0, IntPtr.Zero);
                    MouseEvent(0, 0, (uint)MouseEventF.RightUp, 0, 0, IntPtr.Zero);
                    break;
            }

            if (RepeatAmount > 0)
            {
                clickCount++;
            }

            int randomClickOffset = random.Next(0, ClickOffset);
            await Task.Delay(ClickInterval + randomClickOffset);
        }
    }

    private static void MouseEvent(int dx, int dy, uint dwFlags, uint dwData, uint time, nint dwExtraInfo)
    {
        Input[] inputs = new Input[2];
        inputs[0] = MouseInput(dx, dy, dwData, dwFlags, time, dwExtraInfo);
        inputs[1] = MouseInput(dx, dy, dwData, dwFlags, time, dwExtraInfo);
        _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Input>());
    }

    private static Input MouseInput(int dx, int dy, uint mouseData, uint dwFlags, uint time, nint dwExtraInfo)
    {
        return new Input
        {
            type = 0,
            mi = new InputMouse
            {
                dx = dx,
                dy = dy,
                mouseData = mouseData,
                dwFlags = dwFlags,
                time = time,
                dwExtraInfo = dwExtraInfo
            }
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public int type;
        public InputMouse mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct InputMouse
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [Flags]
    private enum MouseEventF : uint
    {
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040
    }
}