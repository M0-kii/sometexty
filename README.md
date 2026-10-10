# SomeTexty

[![CI](https://github.com/M0-kii/sometexty/actions/workflows/ci.yml/badge.svg)](https://github.com/M0-kii/sometexty/actions/workflows/ci.yml)

A small Windows tray app for everyday text edits. Click the tray icon or press
`Ctrl+Shift+Space` to open it.

## Download

There isn't a release yet. For now, build the app from source using the commands
below. The publish command produces a standalone executable that doesn't need
.NET installed on the machine running it.

## Status

Still in early development.

## Features

- **Autocorrect:** offline English spelling suggestions, underlined mistakes,
  and a personal dictionary. Fix all uses the first suggestion for each word;
  `Ctrl+Z` undoes the change.
- **Case:** upper, lower, title, and sentence case.
- **Count:** word, character, and line counts as you type.
- **Clean:** trim lines, collapse spaces, remove blank lines, or join lines.
- **Replace:** find and replace text, with an option to match case.

The app stays out of the taskbar and Alt+Tab. By default, it hides when you click
away and fills an empty tool with clipboard text when you open it. You can turn
either behavior off in Settings, enable launch at sign-in, and remove words from
your personal dictionary.

## Requirements

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (to build from source)

## Build and run

```sh
dotnet run
```

Run the tests:

```sh
dotnet test tests/SomeTexty.Tests
```

To produce a standalone executable:

```sh
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The result is in `bin/Release/net8.0-windows/win-x64/publish/`.

## Adding a tool

Implement `ITextTool` and add the tool to `Tools/ToolRegistry.cs`. The panel creates
each view once and keeps it between tab switches. Implement `IClipboardAware` on
the view if it should accept clipboard text when opened.

## License

GPL-3.0-or-later. See [LICENSE](LICENSE).

The bundled English dictionary is derived from [SCOWL](http://wordlist.sourceforge.net)
and is distributed under its own permissive license. See
[Dictionaries/en_US-LICENSE.txt](Dictionaries/en_US-LICENSE.txt).
