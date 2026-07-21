# SomeTexty

[![CI](https://github.com/M0-kii/sometexty/actions/workflows/ci.yml/badge.svg)](https://github.com/M0-kii/sometexty/actions/workflows/ci.yml)

A small Windows tray utility for quick text tools. It lives in the system tray
(including the hidden-icons overflow) and opens a compact panel above the tray
when you click its icon.

## Download

Grab `SomeTexty.exe` from the [latest release](https://github.com/M0-kii/sometexty/releases/latest).
It is self-contained, so it runs without installing .NET. No installer: just run it.

## Status

Early development, but usable. A handful of text tools work, along with a
settings screen and a global hotkey.

## Features

- Runs from the system tray, out of the way
- Borderless panel that anchors above the tray, with a global hotkey
  (`Ctrl+Shift+Space`) to open it from anywhere
- Single instance; stays out of the taskbar and Alt+Tab
- Optionally pastes clipboard text into the active tool when it opens

Tools:

- **Autocorrect** — offline spelling suggestions with inline underlines,
  one-click "fix all" (undoable), and a personal dictionary; casing preserved
- **Case** — upper, lower, title, and sentence case
- **Count** — live word, character, and line counts
- **Clean** — trim lines, collapse spaces, remove blank lines, join lines
- **Replace** — plain find and replace, with an optional match-case switch

Settings let you toggle start-with-Windows, auto-hide, and clipboard paste, and
manage the personal dictionary. New tools plug into the tab shell without
touching the panel.

## Requirements

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (to build from source)

## Build and run

```
dotnet run
```

Run the tests:

```
dotnet test tests/SomeTexty.Tests
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
