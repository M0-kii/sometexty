using System.Text.RegularExpressions;

namespace SomeTexty.Autocorrect;

/// <summary>A word found in some text, with where it sits in the original string.</summary>
public readonly record struct WordToken(string Value, int Start)
{
    public int Length => Value.Length;
}

/// <summary>
/// Pulls words out of text while remembering their positions, so corrections can
/// be spliced back in without disturbing punctuation or spacing. Words may contain
/// internal apostrophes or hyphens (don't, well-known).
/// </summary>
public static partial class TextTokenizer
{
    [GeneratedRegex(@"\p{L}+(?:['’\-]\p{L}+)*", RegexOptions.Compiled)]
    private static partial Regex WordPattern();

    public static IEnumerable<WordToken> Tokenize(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }

        foreach (Match match in WordPattern().Matches(text))
        {
            yield return new WordToken(match.Value, match.Index);
        }
    }
}
