using System.Windows;
using SomeTexty.Autocorrect;

namespace SomeTexty;

public partial class App : Application
{
    // Held for the lifetime of the process so a second launch can detect us.
    private static Mutex? _instanceMutex;

    private TrayIcon? _tray;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _instanceMutex = new Mutex(initiallyOwned: true, name: "SomeTexty.SingleInstance", out bool isFirstInstance);
        if (!isFirstInstance)
        {
            // Already running — the existing instance owns the tray icon.
            Shutdown();
            return;
        }

        _ = SpellEngine.LoadAsync();
        _tray = new TrayIcon();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _tray?.Dispose();
        _instanceMutex?.Dispose();
        base.OnExit(e);
    }
}
