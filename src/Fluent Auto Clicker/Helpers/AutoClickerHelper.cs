using System;
using System.Runtime.InteropServices;
using System.Threading;

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
                MouseEvent((uint)MOUSEEVENTF.LEFTDOWN, 0, 0, 0, 0);
                MouseEvent((uint)MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
            }
            else if (mouseButton == 1)
            {
                MouseEvent((uint)MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, 0);
                MouseEvent((uint)MOUSEEVENTF.MIDDLEUP, 0, 0, 0, 0);
            }
            else if (mouseButton == 2)
            {
                MouseEvent((uint)MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, 0);
                MouseEvent((uint)MOUSEEVENTF.RIGHTUP, 0, 0, 0, 0);
            }

            if (repeatAmount > 0)
            {
                clickCount++;
            }

            await Task.Delay(clickInterval);
        }
    }

    private static void MouseEvent(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo)
    {
        INPUT[] inputs = new INPUT[2];
        inputs[0] = CreateMouseEvent(dwFlags);
        inputs[1] = CreateMouseEvent(dwFlags);

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    private static INPUT CreateMouseEvent(uint flags)
    {
        return new INPUT
        {
            type = 0, // INPUT_MOUSE
            mi = new INPUT_MOUSE
            {
                dwFlags = flags,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            }
        };
    }
}
