using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Fluent_Auto_Clicker.Helpers
{
    public static class AutoClickerHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private static bool _isAutoClickerRunning;
        private static Thread _autoClickerThread;

        public static int RepeatInterval = 1000;

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
            while (_isAutoClickerRunning)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                Thread.Sleep(RepeatInterval);
            }
        }
    }
}
