# SomeTexty

A small Windows tray utility for quick text tools. It lives in the system tray
(including the hidden-icons overflow) and opens a compact panel above the tray
when you click its icon.

## Status

Early development. The tray icon and flyout panel are working. Text tools are
being added next, starting with offline autocorrect.

## Features

- Runs from the system tray, out of the way
- Borderless panel that anchors above the tray and dismisses when it loses focus
- Single instance; stays out of the taskbar and Alt+Tab

### Planned

- Autocorrect with spelling suggestions (offline)
- A tool shell so more text tools can be added over time

## Requirements

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (to build from source)

## Build and run

```
dotnet run
```

To produce a standalone executable:

```
dotnet publish -c Release -r win-x64 --self-contained
```

The result is in `bin/Release/net8.0-windows/win-x64/publish/`.

## License

GPL-3.0-or-later. See [LICENSE](LICENSE).
