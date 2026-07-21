using SomeTexty.Autocorrect;
using Xunit;

namespace SomeTexty.Tests;

public class CommonTyposTests
{
    [Theory]
    [InlineData("teh", "the")]
    [InlineData("recieve", "receive")]
    [InlineData("dont", "don't")]
    public void Known_typos_map_to_their_correction(string typo, string expected)
    {
        Assert.True(CommonTypos.TryGetCorrection(typo, out string correction));
        Assert.Equal(expected, correction);
    }

    [Fact]
    public void Lookup_is_case_insensitive()
    {
        Assert.True(CommonTypos.TryGetCorrection("TEH", out string correction));
        Assert.Equal("the", correction);
    }

    [Fact]
    public void Unknown_word_is_not_matched()
        => Assert.False(CommonTypos.TryGetCorrection("perfectlyfine", out _));
}
