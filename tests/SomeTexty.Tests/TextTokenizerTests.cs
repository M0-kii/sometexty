using SomeTexty.Autocorrect;
using Xunit;

namespace SomeTexty.Tests;

public class TextTokenizerTests
{
    [Fact]
    public void Splits_words_and_records_positions()
    {
        var tokens = TextTokenizer.Tokenize("Hi, there!").ToList();

        Assert.Equal(2, tokens.Count);
        Assert.Equal("Hi", tokens[0].Value);
        Assert.Equal(0, tokens[0].Start);
        Assert.Equal("there", tokens[1].Value);
        Assert.Equal(4, tokens[1].Start);
    }

    [Fact]
    public void Keeps_internal_apostrophes_as_one_word()
    {
        var tokens = TextTokenizer.Tokenize("don't").ToList();

        Assert.Single(tokens);
        Assert.Equal("don't", tokens[0].Value);
    }

    [Fact]
    public void Empty_text_yields_no_tokens()
        => Assert.Empty(TextTokenizer.Tokenize(""));
}
