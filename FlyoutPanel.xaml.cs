using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SomeTexty.Hotkeys;
using SomeTexty.Settings;
using SomeTexty.Tools;

namespace SomeTexty;

public partial class FlyoutPanel : Window
{
    /// <summary>Matches the Border margin in XAML that reserves space for the drop shadow.</summary>
    private const double ShadowMargin = 12;

    /// <summary>Visual gap between the card and the edge of the work area.</summary>
    private const double EdgeGap = 8;

    /// <summary>
    /// A tray click can hide the panel before the toggle arrives. Ignore that
    /// toggle so the same click doesn't reopen it.
    /// </summary>
    private static readonly TimeSpan ReopenSuppression = TimeSpan.FromMilliseconds(250);

    private readonly Stopwatch _sinceHidden = Stopwatch.StartNew();

    private static readonly Brush TabSelectedBackground = CreateFrozen("#FF333333");
    private static readonly Brush TabSelectedForeground = CreateFrozen("#FFE6E6E6");
    private static readonly Brush TabIdleForeground = CreateFrozen("#FF9A9A9A");

    /// <summary>Views are built once per tool and reused so state survives tab switches.</summary>
    private readonly Dictionary<ITextTool, UserControl> _views = new();
    private readonly List<Button> _tabs = new();

    private SettingsView? _settingsView;
    private ITextTool? _activeTool;
    private bool _showingSettings;
    private GlobalHotkey? _hotkey;

    public FlyoutPanel()
    {
        InitializeComponent();
        Deactivated += OnDeactivated;
        BuildTabs();

        // Register the hotkey before the panel is first shown.
        new WindowInteropHelper(this).EnsureHandle();
    }

    private void OnDeactivated(object? sender, EventArgs e)
    {
        if (SettingsStore.Load().HideOnFocusLoss)
        {
            HidePanel();
        }
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        if (_showingSettings)
        {
            ShowTools();
        }
        else
        {
            ShowSettings();
        }
    }

    /// <summary>Opens the panel (if hidden) and switches straight to settings. Used by the tray menu.</summary>
    public void OpenSettings()
    {
        if (!IsVisible)
        {
            ShowPanel();
        }

        ShowSettings();
    }

    private void ShowSettings()
    {
        _settingsView ??= new SettingsView();
        _settingsView.Refresh();

        ToolContent.Content = _settingsView;
        TabStrip.Visibility = Visibility.Collapsed;
        HeaderTitle.Text = "Settings";
        _showingSettings = true;
        FadeInContent();
    }

    private void ShowTools()
    {
        TabStrip.Visibility = Visibility.Visible;
        HeaderTitle.Text = "SomeTexty";
        _showingSettings = false;

        if (_activeTool is not null)
        {
            SelectTool(_activeTool);
        }
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
        _activeTool = tool;

        if (!_views.TryGetValue(tool, out var view))
        {
            view = tool.CreateView();
            _views[tool] = view;
        }

        ToolContent.Content = view;
        FadeInContent();

        foreach (var tab in _tabs)
        {
            bool selected = ReferenceEquals(tab.Tag, tool);
            tab.Background = selected ? TabSelectedBackground : Brushes.Transparent;
            tab.Foreground = selected ? TabSelectedForeground : TabIdleForeground;
        }
    }

    /// <summary>Briefly fades the content area in when its contents change.</summary>
    private void FadeInContent()
    {
        var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(110))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        ToolContent.BeginAnimation(OpacityProperty, fade);
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
        var handle = new WindowInteropHelper(this).Handle;
        NativeMethods.HideFromAltTab(handle);

        _hotkey = new GlobalHotkey(handle, ModifierKeys.Control | ModifierKeys.Shift, Key.Space);
        _hotkey.Pressed += ToggleFromHotkey;
    }

    private void ToggleFromHotkey()
    {
        if (IsVisible)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }
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

        ShowPanel();
    }

    private void ShowPanel()
    {
        MoveToTrayCorner();
        Show();
        Activate();
        PlayOpenAnimation();
        OfferClipboardToActiveTool();
    }

    /// <summary>If enabled, pre-fills the active tool with clipboard text (only if its input is empty).</summary>
    private void OfferClipboardToActiveTool()
    {
        if (!SettingsStore.Load().GrabClipboardOnOpen || ToolContent.Content is not IClipboardAware aware)
        {
            return;
        }

        try
        {
            if (Clipboard.ContainsText())
            {
                aware.OfferClipboardText(Clipboard.GetText());
            }
        }
        catch (ExternalException)
        {
            // Another app may have the clipboard open. Leave the input alone.
        }
    }

    /// <summary>A quick fade-and-rise as the panel appears.</summary>
    private void PlayOpenAnimation()
    {
        var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

        RootCard.BeginAnimation(OpacityProperty,
            new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)) { EasingFunction = ease });

        CardShift.BeginAnimation(TranslateTransform.YProperty,
            new DoubleAnimation(10, 0, TimeSpan.FromMilliseconds(180)) { EasingFunction = ease });
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
    /// Places the panel at the bottom-right of the primary screen's work area.
    /// </summary>
    private void MoveToTrayCorner()
    {
        var work = SystemParameters.WorkArea;
        Left = work.Right - EdgeGap - Width + ShadowMargin;
        Top = work.Bottom - EdgeGap - Height + ShadowMargin;
    }
}
