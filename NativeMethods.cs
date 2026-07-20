using System.Runtime.InteropServices;

namespace SomeTexty;

internal static class NativeMethods
{
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    /// <summary>
    /// Marks a window as a tool window, which keeps it out of Alt+Tab.
    /// ShowInTaskbar="False" alone does not reliably do this.
    /// </summary>
    internal static void HideFromAltTab(IntPtr hWnd)
    {
        var style = (long)GetWindowLongPtr(hWnd, GWL_EXSTYLE);
        SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(style | WS_EX_TOOLWINDOW));
    }
}
