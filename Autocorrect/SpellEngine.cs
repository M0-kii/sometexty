using System.IO;
using System.Reflection;
using SomeTexty.Settings;
using WeCantSpell.Hunspell;

namespace SomeTexty.Autocorrect;

/// <summary>
/// Offline spell checking backed by the bundled en_US Hunspell dictionary.
/// The dictionary is loaded once and shared.
/// </summary>
public sealed class SpellEngine
{
    private const string DictionaryResource = "SomeTexty.Dictionaries.en_US.dic";
    private const string AffixResource = "SomeTexty.Dictionaries.en_US.aff";

    private static readonly Lazy<SpellEngine> Lazy = new(() => new SpellEngine());
    private static readonly Lazy<Task<SpellEngine>> LoadTask = new(() => Task.Run(() => Instance));

    public static SpellEngine Instance => Lazy.Value;

    public static Task<SpellEngine> LoadAsync() => LoadTask.Value;

    private readonly WordList _words;

    // Words the user has added; checked alongside the dictionary. The dictionary
    // itself is immutable, so personal words live in this set instead.
    private readonly HashSet<string> _personalWords;

    private SpellEngine()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var dictStream = OpenResource(assembly, DictionaryResource);
        using var affixStream = OpenResource(assembly, AffixResource);
        _words = WordList.CreateFromStreams(dictStream, affixStream);

        _personalWords = new HashSet<string>(
            SettingsStore.Load().PersonalWords, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>True if the word is in the dictionary or the user's personal list.</summary>
    public bool IsCorrect(string word) => _personalWords.Contains(word) || _words.Check(word);

    /// <summary>Ranked replacement suggestions, best first.</summary>
    public IReadOnlyList<string> Suggest(string word) => _words.Suggest(word).ToList();

    /// <summary>The user's personal words, alphabetically ordered.</summary>
    public IReadOnlyList<string> PersonalWords =>
        _personalWords.OrderBy(w => w, StringComparer.CurrentCultureIgnoreCase).ToList();

    /// <summary>Adds a word to the user's personal list and persists it.</summary>
    public void AddToPersonal(string word)
    {
        if (string.IsNullOrWhiteSpace(word) || !_personalWords.Add(word))
        {
            return;
        }

        var settings = SettingsStore.Load();
        settings.PersonalWords.Add(word);
        SettingsStore.Save(settings);
    }

    /// <summary>Removes a word from the user's personal list and persists the change.</summary>
    public void RemoveFromPersonal(string word)
    {
        if (!_personalWords.Remove(word))
        {
            return;
        }

        var settings = SettingsStore.Load();
        settings.PersonalWords.RemoveAll(w => string.Equals(w, word, StringComparison.OrdinalIgnoreCase));
        SettingsStore.Save(settings);
    }

    /// <summary>Removes all personal words and persists the change.</summary>
    public void ClearPersonal()
    {
        if (_personalWords.Count == 0)
        {
            return;
        }

        _personalWords.Clear();
        var settings = SettingsStore.Load();
        settings.PersonalWords.Clear();
        SettingsStore.Save(settings);
    }

    private static Stream OpenResource(Assembly assembly, string name)
        => assembly.GetManifestResourceStream(name)
           ?? throw new InvalidOperationException($"Embedded resource not found: {name}");
}
