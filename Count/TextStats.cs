using System.Text.RegularExpressions;

namespace SomeTexty.Count;

/// <summary>Basic counts for a block of text.</summary>
public readonly record struct TextStats(int Words, int Characters, int CharactersNoSpaces, int Lines)
{
    private static readonly Regex WordRun = new(@"\S+", RegexOptions.Compiled);

    public static TextStats From(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new TextStats(0, 0, 0, 0);
        }

        int words = WordRun.Matches(text).Count;
        int characters = text.Length;
        int charactersNoSpaces = text.Count(c => !char.IsWhiteSpace(c));
        int lines = text.Split('\n').Length;

        return new TextStats(words, characters, charactersNoSpaces, lines);
    }
}
