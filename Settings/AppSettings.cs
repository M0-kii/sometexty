namespace SomeTexty.Settings;

/// <summary>User settings saved to disk.</summary>
public sealed class AppSettings
{
    /// <summary>Words the user has added so they stop being flagged as misspelled.</summary>
    public List<string> PersonalWords { get; set; } = new();

    /// <summary>When true, the panel hides as soon as it loses focus (the default).</summary>
    public bool HideOnFocusLoss { get; set; } = true;

    /// <summary>When true, the active tool is pre-filled with clipboard text on open (the default).</summary>
    public bool GrabClipboardOnOpen { get; set; } = true;
}
