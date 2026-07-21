using SomeTexty.Autocorrect;
using Xunit;

namespace SomeTexty.Tests;

public class CorrectionServiceTests
{
    private readonly CorrectionService _service = new(SpellEngine.Instance);

    [Theory]
    [InlineData("TEH", "the", "THE")]
    [InlineData("Teh", "the", "The")]
    [InlineData("teh", "the", "the")]
    [InlineData("i", "I", "I")]
    public void MatchCase_follows_the_original_casing(string original, string replacement, string expected)
        => Assert.Equal(expected, CorrectionService.MatchCase(original, replacement));

    [Fact]
    public void ReplaceWord_preserves_each_occurrences_case()
        => Assert.Equal("The the THE", _service.ReplaceWord("Teh teh TEH", "teh", "the"));

    [Fact]
    public void ReplaceWord_only_matches_whole_words()
        => Assert.Equal("theatre", _service.ReplaceWord("theatre", "the", "XXX"));

    [Fact]
    public void Analyze_flags_a_common_misspelling_with_correct_first_suggestion()
    {
        var results = _service.Analyze("recieve");

        var hit = Assert.Single(results);
        Assert.Equal("recieve", hit.Word);
        Assert.Equal("receive", hit.Suggestions[0]);
    }

    [Fact]
    public void Analyze_returns_nothing_for_correct_text()
        => Assert.Empty(_service.Analyze("the quick brown fox"));
}
