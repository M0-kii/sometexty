using SomeTexty.Replace;
using Xunit;

namespace SomeTexty.Tests;

public class TextReplacerTests
{
    [Fact]
    public void ReplaceAll_replaces_every_occurrence()
        => Assert.Equal("b-b-b", TextReplacer.ReplaceAll("a-a-a", "a", "b", caseSensitive: false));

    [Fact]
    public void ReplaceAll_ignores_case_when_asked()
        => Assert.Equal("x x", TextReplacer.ReplaceAll("A a", "a", "x", caseSensitive: false));

    [Fact]
    public void ReplaceAll_respects_case_when_sensitive()
        => Assert.Equal("A x", TextReplacer.ReplaceAll("A a", "a", "x", caseSensitive: true));

    [Fact]
    public void ReplaceAll_with_empty_find_is_unchanged()
        => Assert.Equal("abc", TextReplacer.ReplaceAll("abc", "", "x", caseSensitive: false));

    [Fact]
    public void CountOccurrences_counts_non_overlapping()
        => Assert.Equal(2, TextReplacer.CountOccurrences("aaaa", "aa", caseSensitive: true));
}
