using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace SomeTexty.Hotkeys;

/// <summary>
/// Registers a global hotkey. Check <see cref="IsRegistered"/> for conflicts.
/// </summary>
public sealed class GlobalHotkey : IDisposable
{
    private const int WmHotkey = 0x0312;
    private const int HotkeyId = 0x4A01;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private readonly IntPtr _hwnd;
    private readonly HwndSource _source;

    public event Action? Pressed;

    public bool IsRegistered { get; }

    public GlobalHotkey(IntPtr hwnd, ModifierKeys modifiers, Key key)
    {
        _hwnd = hwnd;
        _source = HwndSource.FromHwnd(hwnd) ?? throw new InvalidOperationException("Window handle has no HwndSource.");
        _source.AddHook(WndProc);

        uint vk = (uint)KeyInterop.VirtualKeyFromKey(key);
        IsRegistered = RegisterHotKey(hwnd, HotkeyId, (uint)modifiers, vk);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WmHotkey && wParam.ToInt32() == HotkeyId)
        {
            Pressed?.Invoke();
            handled = true;
        }

        return IntPtr.Zero;
    }

    public void Dispose()
    {
        _source.RemoveHook(WndProc);
        if (IsRegistered)
        {
            UnregisterHotKey(_hwnd, HotkeyId);
        }
    }
}
