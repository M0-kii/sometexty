using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SomeTexty;

public partial class FlyoutPanel : Window
{
    /// <summary>Matches the Border margin in XAML that reserves space for the drop shadow.</summary>
    private const double ShadowMargin = 12;

    /// <summary>Visual gap between the card and the edge of the work area.</summary>
    private const double EdgeGap = 8;

    /// <summary>
    /// Clicking the tray icon while the panel is open deactivates it (hiding it)
    /// and then fires the toggle, which would immediately reopen it. Ignoring
    /// toggles that arrive right after a hide makes the icon close the panel.
    /// </summary>
    private static readonly TimeSpan ReopenSuppression = TimeSpan.FromMilliseconds(250);

    private readonly Stopwatch _sinceHidden = Stopwatch.StartNew();

    public FlyoutPanel()
    {
        InitializeComponent();
        Deactivated += (_, _) => HidePanel();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        NativeMethods.HideFromAltTab(new WindowInteropHelper(this).Handle);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            HidePanel();
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    /// <summary>The panel is hidden and reused, never actually closed, until the app exits.</summary>
    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        HidePanel();
    }

    public void Toggle()
    {
        if (IsVisible)
        {
            HidePanel();
            return;
        }

        if (_sinceHidden.Elapsed < ReopenSuppression)
        {
            return;
        }

        MoveToTrayCorner();
        Show();
        Activate();
    }

    private void HidePanel()
    {
        if (!IsVisible)
        {
            return;
        }

        Hide();
        _sinceHidden.Restart();
    }

    /// <summary>
    /// Anchors the card just inside the corner of the work area. WorkArea already
    /// excludes the taskbar, so this lands above the tray wherever the taskbar sits.
    /// </summary>
    private void MoveToTrayCorner()
    {
        var work = SystemParameters.WorkArea;
        Left = work.Right - EdgeGap - Width + ShadowMargin;
        Top = work.Bottom - EdgeGap - Height + ShadowMargin;
    }
}
