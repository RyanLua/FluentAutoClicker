using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Fluent_Auto_Clicker.Helpers;

public static class AutoClickerHelper
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    // DWORD dwFlags for MOUSEINPUT
    private const int MOUSEEVENTF_LEFTDOWN = 0x0002; // The left button was pressed.
    private const int MOUSEEVENTF_LEFTUP = 0x0004; // The left button was released.
    private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; // The right button was pressed.
    private const int MOUSEEVENTF_RIGHTUP = 0x0010; // The right button was released.
    private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; // The middle button was pressed.
    private const int MOUSEEVENTF_MIDDLEUP = 0x0040; // The middle button was pressed.

    // Internal states
    private static bool _isAutoClickerRunning;

    // Public fields
    public static int RepeatInterval = 1000;
    public static int RepeatCount = 0;
    public static int MouseButton = 0; // 0 = Left, 1 = Middle, 2 = Right

    public static bool IsAutoClickerRunning => _isAutoClickerRunning;

    // Start the auto clicker
    public static void StartAutoClicker()
    {
        Debug.Print("AutoClickerHelper: Starting...");
        if (!_isAutoClickerRunning)
        {
            _isAutoClickerRunning = true;
            ClickAuto(RepeatCount);
            Debug.Print("AutoClickerHelper: Started");
        }
        else
        {
            Debug.Print("AutoClickerHelper: Failed to start, already started");
        }
    }

    // Stop the auto clicker
    public static void StopAutoClicker()
    {
        Debug.Print("AutoClickerHelper: Stopping...");
        if (_isAutoClickerRunning)
        {
            _isAutoClickerRunning = false;
            Debug.Print("AutoClickerHelper: Stopped");
        }
        else
        {
            Debug.Print("AutoClickerHelper: Failed to stop, already stopped");
        }
    }

    // Click the mouse using mouse_event
    private static void ClickMouse(int Button)
    {
        if (Button == 0)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        else if (Button == 1)
        {
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }
        else
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
    }

    // Loop to handle the clicking
    private static void ClickAuto(int ClickAmount)
    {
        var ClickCount = 0;

        while (_isAutoClickerRunning && (RepeatCount <= 0 || ClickCount < ClickAmount))
        {
            ClickMouse(MouseButton);

            if (RepeatCount > 0)
            {
                ClickCount++;
            }

            Task.Delay(RepeatInterval);
        }
    }
}