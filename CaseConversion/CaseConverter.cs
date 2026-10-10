using System.Text.RegularExpressions;

namespace SomeTexty.CaseConversion;

/// <summary>Text case transformations used by the Case tool.</summary>
public static partial class CaseConverter
{
    // First letter of each word (start of text or after whitespace).
    [GeneratedRegex(@"(?<=^|\s)\p{L}")]
    private static partial Regex WordStart();

    // Keep the punctuation and whitespace before each sentence.
    [GeneratedRegex(@"(^|[.!?]\s+)(\p{L})")]
    private static partial Regex SentenceStart();

    public static string ToUpper(string text) => text.ToUpperInvariant();

    public static string ToLower(string text) => text.ToLowerInvariant();

    public static string ToTitle(string text)
        => WordStart().Replace(text.ToLowerInvariant(), m => m.Value.ToUpperInvariant());

    public static string ToSentence(string text)
        => SentenceStart().Replace(text.ToLowerInvariant(),
            m => m.Groups[1].Value + m.Groups[2].Value.ToUpperInvariant());
}
