using SomeTexty.Count;
using Xunit;

namespace SomeTexty.Tests;

public class TextStatsTests
{
    [Fact]
    public void Counts_words_characters_and_lines()
    {
        var stats = TextStats.From("hello world");

        Assert.Equal(2, stats.Words);
        Assert.Equal(11, stats.Characters);
        Assert.Equal(10, stats.CharactersNoSpaces);
        Assert.Equal(1, stats.Lines);
    }

    [Fact]
    public void Empty_text_is_all_zero()
    {
        var stats = TextStats.From("");

        Assert.Equal(0, stats.Words);
        Assert.Equal(0, stats.Characters);
        Assert.Equal(0, stats.CharactersNoSpaces);
        Assert.Equal(0, stats.Lines);
    }

    [Fact]
    public void Counts_multiple_lines()
        => Assert.Equal(3, TextStats.From("a\nb\nc").Lines);
}
