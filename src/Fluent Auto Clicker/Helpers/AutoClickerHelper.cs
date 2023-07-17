using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Fluent_Auto_Clicker.Helpers;

public static class AutoClickerHelper
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const int MOUSEEVENTF_LEFTUP = 0x0004;
    private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const int MOUSEEVENTF_RIGHTUP = 0x0010;

    private static bool _isAutoClickerRunning;
    private static Thread _autoClickerThread;

    public static int RepeatInterval = 1000;
    public static int RepeatCount = 0;
    public static int MouseButton = 0; // 0 = Left, 1 = Middle, 2 = Right

    public static bool IsAutoClickerRunning => _isAutoClickerRunning;

    public static void StartAutoClicker()
    {
        if (!_isAutoClickerRunning)
        {
            _isAutoClickerRunning = true;
            _autoClickerThread = new Thread(AutoClickerThread);
            _autoClickerThread.Start();
        }
    }

    public static void StopAutoClicker()
    {
        if (_isAutoClickerRunning)
        {
            _isAutoClickerRunning = false;
            _autoClickerThread.Join();
        }
    }

    private static void AutoClickerThread()
    {
        var clickCount = 0;

        while (_isAutoClickerRunning && (RepeatCount <= 0 || clickCount < RepeatCount))
        {
            if (MouseButton == 0)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            else if (MouseButton == 1)
            {
                mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
            }
            else
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            }

            if (RepeatCount > 0)
            {
                clickCount++;
            }

            Thread.Sleep(RepeatInterval);
        }
    }
}
