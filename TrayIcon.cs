using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;

namespace SomeTexty;

/// <summary>
/// Owns the system tray icon and its menu. Windows places this in the
/// "hidden icons" overflow by default; dragging it out is handled by Windows.
/// </summary>
public sealed class TrayIcon : IDisposable
{
    private readonly TaskbarIcon _icon;
    private readonly FlyoutPanel _panel = new();

    public TrayIcon()
    {
        _icon = new TaskbarIcon
        {
            IconSource = new BitmapImage(new Uri("pack://application:,,,/Assets/tray.ico")),
            ToolTipText = "SomeTexty",
            ContextMenu = BuildMenu(),
        };

        _icon.TrayLeftMouseUp += (_, _) => TogglePanel();
    }

    private ContextMenu BuildMenu()
    {
        var menu = new ContextMenu();

        var open = new MenuItem { Header = "Open" };
        open.Click += (_, _) => TogglePanel();
        menu.Items.Add(open);

        var settings = new MenuItem { Header = "Settings" };
        settings.Click += (_, _) => _panel.OpenSettings();
        menu.Items.Add(settings);

        menu.Items.Add(new Separator());

        var quit = new MenuItem { Header = "Quit" };
        quit.Click += (_, _) => Application.Current.Shutdown();
        menu.Items.Add(quit);

        return menu;
    }

    private void TogglePanel() => _panel.Toggle();

    public void Dispose() => _icon.Dispose();
}
