using System.Text.RegularExpressions;

namespace SomeTexty.Clean;

/// <summary>Whitespace clean-up operations for text.</summary>
public static partial class TextCleaner
{
    [GeneratedRegex(@"[ \t]{2,}")]
    private static partial Regex RepeatedSpaces();

    [GeneratedRegex(@"\s+")]
    private static partial Regex AnyWhitespace();

    /// <summary>Trims leading and trailing whitespace from each line.</summary>
    public static string TrimLines(string text)
        => string.Join("\n", SplitLines(text).Select(line => line.Trim()));

    /// <summary>Collapses runs of spaces or tabs into a single space.</summary>
    public static string CollapseSpaces(string text)
        => RepeatedSpaces().Replace(text, " ");

    /// <summary>Removes lines that are empty or whitespace only.</summary>
    public static string RemoveBlankLines(string text)
        => string.Join("\n", SplitLines(text).Where(line => line.Trim().Length > 0));

    /// <summary>Joins everything into a single line, collapsing all whitespace to single spaces.</summary>
    public static string JoinLines(string text)
        => AnyWhitespace().Replace(text, " ").Trim();

    private static string[] SplitLines(string text)
        => text.Replace("\r\n", "\n").Split('\n');
}
