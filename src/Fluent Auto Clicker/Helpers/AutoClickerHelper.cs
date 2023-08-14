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
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private const int INPUT_MOUSE = 0;
    private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const int MOUSEEVENTF_LEFTUP = 0x0004;
    private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const int MOUSEEVENTF_RIGHTUP = 0x0010;

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
                MouseEvent(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                MouseEvent(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            else if (mouseButton == 1)
            {
                MouseEvent(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                MouseEvent(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
            }
            else if (mouseButton == 2)
            {
                MouseEvent(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                MouseEvent(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
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
            type = INPUT_MOUSE,
            mi = new MOUSEINPUT
            {
                dwFlags = flags,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            }
        };
    }
}
