# SomeTexty

A small Windows tray utility for quick text tools. It lives in the system tray
(including the hidden-icons overflow) and opens a compact panel above the tray
when you click its icon.

## Status

Early development, but usable. The tray app, flyout panel, and an offline
autocorrect tool are working.

## Features

- Runs from the system tray, out of the way
- Borderless panel that anchors above the tray and dismisses when it loses focus
- Single instance; stays out of the taskbar and Alt+Tab
- Offline autocorrect: spelling suggestions, one-click "fix all", and copy, with
  original casing preserved
- A tab-based tool shell, so new text tools can be added without touching the panel

### Planned

- Case conversion tools
- Personal dictionary and saved settings
- Inline underlining of misspelled words in the text box

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

The bundled English dictionary is derived from [SCOWL](http://wordlist.sourceforge.net)
and is distributed under its own permissive license. See
[Dictionaries/en_US-LICENSE.txt](Dictionaries/en_US-LICENSE.txt).
