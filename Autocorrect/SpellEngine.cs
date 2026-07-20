using System.IO;
using System.Reflection;
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

    public static SpellEngine Instance => Lazy.Value;

    private readonly WordList _words;

    private SpellEngine()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var dictStream = OpenResource(assembly, DictionaryResource);
        using var affixStream = OpenResource(assembly, AffixResource);
        _words = WordList.CreateFromStreams(dictStream, affixStream);
    }

    /// <summary>True if the word is spelled correctly.</summary>
    public bool IsCorrect(string word) => _words.Check(word);

    /// <summary>Ranked replacement suggestions, best first.</summary>
    public IReadOnlyList<string> Suggest(string word) => _words.Suggest(word).ToList();

    private static Stream OpenResource(Assembly assembly, string name)
        => assembly.GetManifestResourceStream(name)
           ?? throw new InvalidOperationException($"Embedded resource not found: {name}");
}
