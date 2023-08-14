using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;

namespace Fluent_Auto_Clicker.Helpers;

public static class AutoClickerHelper
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public int type;
        public INPUT_MOUSE mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT_MOUSE
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [Flags]
    internal enum MOUSEEVENTF : uint
    {
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
    }

    private static bool _isAutoClickerRunning;
    private static Thread? _autoClickerThread;

    public static int clickInterval = 1000; // Milliseconds
    public static int repeatAmount = 0; // How many clicks
    public static int mouseButton = 0; // 0 = Left, 1 = Middle, 2 = Right

    public static bool IsAutoClickerRunning => _isAutoClickerRunning;

    public static void StartAutoClicker()
    {
        _isAutoClickerRunning = true;
        _autoClickerThread = new Thread(AutoClickerThread);
        _autoClickerThread.Start();
    }

    public static void StopAutoClicker()
    {
        _isAutoClickerRunning = false;
        _autoClickerThread?.Join();
    }

    private static async void AutoClickerThread()
    {
        var clickCount = 0;

        while (_isAutoClickerRunning)
        {
            if (clickCount >= repeatAmount && repeatAmount != 0)
            {
                StopAutoClicker();
                break;
            }

            if (mouseButton == 0)
            {
                MouseEvent(0, 0, (uint)MOUSEEVENTF.LEFTDOWN, 0, 0, IntPtr.Zero);
                MouseEvent(0, 0, (uint)MOUSEEVENTF.LEFTUP, 0, 0, IntPtr.Zero);
            }
            else if (mouseButton == 1)
            {
                MouseEvent(0, 0, (uint)MOUSEEVENTF.MIDDLEDOWN, 0, 0, IntPtr.Zero);
                MouseEvent(0, 0, (uint)MOUSEEVENTF.MIDDLEUP, 0, 0, IntPtr.Zero);
            }
            else if (mouseButton == 2)
            {
                MouseEvent(0, 0, (uint)MOUSEEVENTF.RIGHTDOWN, 0, 0, IntPtr.Zero);
                MouseEvent(0, 0, (uint)MOUSEEVENTF.RIGHTUP, 0, 0, IntPtr.Zero);
            }

            if (repeatAmount > 0)
            {
                clickCount++;
            }

            await Task.Delay(clickInterval);
        }
    }

    private static void MouseEvent(int dx, int dy, uint dwFlags, uint dwData, uint time, nint dwExtraInfo)
    {
        INPUT[] inputs = new INPUT[2];
        inputs[0] = MOUSEINPUT(dx, dy, dwData, dwFlags, time, dwExtraInfo);
        inputs[1] = MOUSEINPUT(dx, dy, dwData, dwFlags, time, dwExtraInfo);

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    private static INPUT MOUSEINPUT(int DX, int DY, uint MOUSEDATA, uint DWFLAGS, uint TIME, nint DWEXTRAINFO)
    {
        return new INPUT
        {
            type = 0, // INPUT_MOUSE
            mi = new INPUT_MOUSE
            {
                dx = DX,
                dy = DY,
                mouseData = MOUSEDATA,
                dwFlags = DWFLAGS,
                time = TIME,
                dwExtraInfo = DWEXTRAINFO
            }
        };
    }
}
