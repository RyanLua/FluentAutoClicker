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

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WinRT;

namespace FluentAutoClicker;

/// <summary>
///     Customized <c>Program.cs</c> file to implement
///     <see
///         href="https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/applifecycle/applifecycle-single-instance">
///         single-instancing
///         in a WinUI app with C#.
///     </see>
///     Single-instanced apps only allow one instance of the app running at a time.
/// </summary>
public partial class Program
{
    private static IntPtr _redirectEventHandle = IntPtr.Zero;

    [STAThread]
    private static int Main()
    {
        ComWrappersSupport.InitializeComWrappers();
        bool isRedirect = DecideRedirection();

        if (!isRedirect)
        {
            Application.Start(p =>
            {
                DispatcherQueueSynchronizationContext context = new(
                    DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = new App();
            });
        }

        return 0;
    }

    private static bool DecideRedirection()
    {
        bool isRedirect = false;
        AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
        ExtendedActivationKind kind = args.Kind;
        AppInstance keyInstance = AppInstance.FindOrRegisterForKey("FluentAutoClickerApp");

        if (keyInstance.IsCurrent)
        {
            keyInstance.Activated += OnActivated;
        }
        else
        {
            isRedirect = true;
            RedirectActivationTo(args, keyInstance);
        }

        return isRedirect;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "CreateEventW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr CreateEvent(
        IntPtr lpEventAttributes, [MarshalAs(UnmanagedType.Bool)] bool bManualReset,
        [MarshalAs(UnmanagedType.Bool)] bool bInitialState, string? lpName);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetEvent(IntPtr hEvent);

    [LibraryImport("ole32.dll")]
    private static partial uint CoWaitForMultipleObjects(
        uint dwFlags, uint dwMilliseconds, ulong nHandles,
        [In, Out] IntPtr[] pHandles, out uint dwIndex);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(IntPtr hWnd);

    // Do the redirection on another thread, and use a non-blocking
    // wait method to wait for the redirection to complete.
    public static void RedirectActivationTo(AppActivationArguments args,
        AppInstance keyInstance)
    {
        _redirectEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
        _ = Task.Run(() =>
        {
            keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
            _ = SetEvent(_redirectEventHandle);
        });

        uint cwmoDefault = 0;
        uint infinite = 0xFFFFFFFF;
        _ = CoWaitForMultipleObjects(
            cwmoDefault, infinite, 1,
            [_redirectEventHandle], out uint handleIndex);

        // Bring the window to the foreground
        Process process = Process.GetProcessById((int)keyInstance.ProcessId);
        _ = SetForegroundWindow(process.MainWindowHandle);
    }

    private static void OnActivated(object? sender, AppActivationArguments args)
    {
        _ = args.Kind;
    }
}