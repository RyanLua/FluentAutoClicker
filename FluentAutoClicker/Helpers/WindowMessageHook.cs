﻿/*
  Copyright (C) 2024 Ryan Luu

  This file is part of Fluent Auto Clicker.

  Fluent Auto Clicker is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  Fluent Auto Clicker is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with Fluent Auto Clicker. If not, see <https://www.gnu.org/licenses/>.
*/

using Microsoft.UI.Xaml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace FluentAutoClicker.Helpers;

public class WindowMessageHook : IEquatable<WindowMessageHook>, IDisposable
{
    private delegate nint SUBCLASSPROC(nint hWnd, uint uMsg, nint wParam, nint lParam, nint uIdSubclass, uint dwRefData);

    private static readonly ConcurrentDictionary<nint, WindowMessageHook> _hooks = new();
    private static readonly SUBCLASSPROC _proc = SubclassProc;

    public event EventHandler<MessageEventArgs> Message;
    private nint _hWnd;

    public WindowMessageHook(Window window) : this(GetHandle(window)) { }
    public WindowMessageHook(nint hWnd)
    {
        if (hWnd == 0)
            throw new ArgumentException(null, nameof(hWnd));

        _hWnd = hWnd;
        _hooks.AddOrUpdate(hWnd, this, (k, o) =>
        {
            if (Equals(o)) return o;
            o.Dispose();
            return this;
        });
        if (!SetWindowSubclass(hWnd, _proc, 0, 0))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    protected virtual void OnMessage(object sender, MessageEventArgs e) => Message?.Invoke(sender, e);
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        var hWnd = Interlocked.Exchange(ref _hWnd, IntPtr.Zero);
        if (hWnd != IntPtr.Zero)
        {
            RemoveWindowSubclass(hWnd, _proc, 0);
            _hooks.Remove(hWnd, out _);
        }
    }

    ~WindowMessageHook() { Dispose(disposing: false); }
    public void Dispose() { Dispose(disposing: true); GC.SuppressFinalize(this); }

    [DllImport("comctl32", SetLastError = true)]
    private static extern bool SetWindowSubclass(nint hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

    [DllImport("comctl32", SetLastError = true)]
    private static extern nint DefSubclassProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

    [DllImport("comctl32", SetLastError = true)]
    private static extern bool RemoveWindowSubclass(nint hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass);

    private static nint GetHandle(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        return WinRT.Interop.WindowNative.GetWindowHandle(window);
    }

    private static nint SubclassProc(nint hWnd, uint uMsg, nint wParam, nint lParam, nint uIdSubclass, uint dwRefData)
    {
        if (_hooks.TryGetValue(hWnd, out var hook))
        {
            var e = new MessageEventArgs(hWnd, uMsg, wParam, lParam);
            hook.OnMessage(hook, e);
            if (e.Result.HasValue)
                return e.Result.Value;
        }
        return DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    public override int GetHashCode() => _hWnd.GetHashCode();
    public override string ToString() => _hWnd.ToString();
    public override bool Equals(object obj) => Equals(obj as WindowMessageHook);
    public virtual bool Equals(WindowMessageHook other) => other != null && _hWnd.Equals(other._hWnd);
}

public class MessageEventArgs : EventArgs
{
    public MessageEventArgs(nint hWnd, uint uMsg, nint wParam, nint lParam)
    {
        HWnd = hWnd;
        Message = uMsg;
        WParam = wParam;
        LParam = lParam;
    }

    public nint HWnd { get; }
    public uint Message { get; }
    public nint WParam { get; }
    public nint LParam { get; }
    public virtual nint? Result { get; set; }
}
