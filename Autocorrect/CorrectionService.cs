using System.Text.RegularExpressions;

namespace SomeTexty.Autocorrect;

/// <summary>A misspelled word and the suggested replacements for it.</summary>
public sealed record Misspelling(string Word, IReadOnlyList<string> Suggestions);

/// <summary>
/// Finds misspellings and preserves casing when replacing words.
/// </summary>
public sealed class CorrectionService
{
    private readonly SpellEngine _engine;

    public CorrectionService(SpellEngine engine) => _engine = engine;

    /// <summary>Adds a word to the personal dictionary so it stops being flagged.</summary>
    public void AddToDictionary(string word) => _engine.AddToPersonal(word);

    /// <summary>
    /// Positions of all misspelled words, including repeated occurrences.
    /// </summary>
    public IEnumerable<(int Start, int Length)> FindMisspelledSpans(string text)
    {
        foreach (var token in TextTokenizer.Tokenize(text))
        {
            if (!_engine.IsCorrect(token.Value))
            {
                yield return (token.Start, token.Length);
            }
        }
    }

    /// <summary>
    /// Distinct misspellings in order of appearance, ignoring case.
    /// </summary>
    public IReadOnlyList<Misspelling> Analyze(string text)
    {
        var results = new List<Misspelling>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var token in TextTokenizer.Tokenize(text))
        {
            if (_engine.IsCorrect(token.Value) || !seen.Add(token.Value))
            {
                continue;
            }

            results.Add(new Misspelling(token.Value, BuildSuggestions(token.Value)));
        }

        return results;
    }

    /// <summary>
    /// Known typo fixes first, followed by Hunspell suggestions without duplicates.
    /// </summary>
    private List<string> BuildSuggestions(string word)
    {
        var suggestions = new List<string>();

        if (CommonTypos.TryGetCorrection(word, out var fix))
        {
            suggestions.Add(fix);
        }

        foreach (var suggestion in _engine.Suggest(word))
        {
            if (!suggestions.Contains(suggestion, StringComparer.OrdinalIgnoreCase))
            {
                suggestions.Add(suggestion);
            }
        }

        return suggestions;
    }

    /// <summary>
    /// Replaces every whole-word occurrence of <paramref name="word"/> (any casing)
    /// with <paramref name="replacement"/>, matching each occurrence's original case.
    /// </summary>
    public string ReplaceWord(string text, string word, string replacement)
    {
        var pattern = $@"\b{Regex.Escape(word)}\b";
        return Regex.Replace(
            text,
            pattern,
            match => MatchCase(match.Value, replacement),
            RegexOptions.IgnoreCase);
    }

    /// <summary>Reshapes <paramref name="replacement"/> to follow the casing of <paramref name="original"/>.</summary>
    internal static string MatchCase(string original, string replacement)
    {
        if (string.IsNullOrEmpty(replacement))
        {
            return replacement;
        }

        // ALL CAPS -> ALL CAPS.
        if (original.Length > 1 && original.All(char.IsUpper))
        {
            return replacement.ToUpperInvariant();
        }

        // Titlecase -> Titlecase. Otherwise leave the suggestion's own casing,
        // which lets fixes like "i" -> "I" come through unchanged.
        if (char.IsUpper(original[0]))
        {
            return char.ToUpperInvariant(replacement[0]) + replacement[1..];
        }

        return replacement;
    }
}
