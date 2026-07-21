using SomeTexty.Clean;
using Xunit;

namespace SomeTexty.Tests;

public class TextCleanerTests
{
    [Fact]
    public void TrimLines_trims_each_line()
        => Assert.Equal("a\nb", TextCleaner.TrimLines("  a  \n  b  "));

    [Fact]
    public void CollapseSpaces_reduces_runs_to_one()
        => Assert.Equal("a b c", TextCleaner.CollapseSpaces("a    b  c"));

    [Fact]
    public void RemoveBlankLines_drops_empty_lines()
        => Assert.Equal("a\nb", TextCleaner.RemoveBlankLines("a\n\n   \nb"));

    [Fact]
    public void JoinLines_produces_a_single_line()
        => Assert.Equal("a b c", TextCleaner.JoinLines("a\n b\tc"));
}
