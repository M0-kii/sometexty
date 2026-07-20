using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using SomeTexty.Tools;

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

    private static readonly Brush TabSelectedBackground = CreateFrozen("#FF333333");
    private static readonly Brush TabSelectedForeground = CreateFrozen("#FFE6E6E6");
    private static readonly Brush TabIdleForeground = CreateFrozen("#FF9A9A9A");

    /// <summary>Views are built once per tool and reused so state survives tab switches.</summary>
    private readonly Dictionary<ITextTool, UserControl> _views = new();
    private readonly List<Button> _tabs = new();

    public FlyoutPanel()
    {
        InitializeComponent();
        Deactivated += (_, _) => HidePanel();
        BuildTabs();
    }

    private void BuildTabs()
    {
        var tabStyle = (Style)Resources["TabButton"];

        foreach (var tool in ToolRegistry.Tools)
        {
            var tab = new Button { Style = tabStyle, Content = tool.Name, Tag = tool };
            tab.Click += (_, _) => SelectTool(tool);
            TabStrip.Children.Add(tab);
            _tabs.Add(tab);
        }

        if (ToolRegistry.Tools.Count > 0)
        {
            SelectTool(ToolRegistry.Tools[0]);
        }
    }

    private void SelectTool(ITextTool tool)
    {
        if (!_views.TryGetValue(tool, out var view))
        {
            view = tool.CreateView();
            _views[tool] = view;
        }

        ToolContent.Content = view;

        foreach (var tab in _tabs)
        {
            bool selected = ReferenceEquals(tab.Tag, tool);
            tab.Background = selected ? TabSelectedBackground : Brushes.Transparent;
            tab.Foreground = selected ? TabSelectedForeground : TabIdleForeground;
        }
    }

    private static Brush CreateFrozen(string hex)
    {
        var brush = (Brush)new BrushConverter().ConvertFromString(hex)!;
        brush.Freeze();
        return brush;
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
