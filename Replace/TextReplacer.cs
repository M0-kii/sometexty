namespace SomeTexty.Replace;

/// <summary>Plain find-and-replace over text.</summary>
public static class TextReplacer
{
    /// <summary>
    /// Replaces every occurrence of <paramref name="find"/> with <paramref name="replacement"/>.
    /// An empty search string leaves the text unchanged.
    /// </summary>
    public static string ReplaceAll(string text, string find, string replacement, bool caseSensitive)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(find))
        {
            return text;
        }

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return text.Replace(find, replacement, comparison);
    }

    /// <summary>Counts how many non-overlapping occurrences of <paramref name="find"/> are present.</summary>
    public static int CountOccurrences(string text, string find, bool caseSensitive)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(find))
        {
            return 0;
        }

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(find, index, comparison)) >= 0)
        {
            count++;
            index += find.Length;
        }

        return count;
    }
}
