using SomeTexty.CaseConversion;
using Xunit;

namespace SomeTexty.Tests;

public class CaseConverterTests
{
    [Fact]
    public void ToUpper_uppercases_everything()
        => Assert.Equal("HELLO WORLD", CaseConverter.ToUpper("Hello World"));

    [Fact]
    public void ToLower_lowercases_everything()
        => Assert.Equal("hello world", CaseConverter.ToLower("Hello World"));

    [Fact]
    public void ToTitle_capitalizes_each_word()
        => Assert.Equal("Hello World", CaseConverter.ToTitle("hello world"));

    [Fact]
    public void ToTitle_does_not_capitalize_after_apostrophe()
        => Assert.Equal("Don't Stop", CaseConverter.ToTitle("don't stop"));

    [Fact]
    public void ToSentence_capitalizes_after_terminators()
        => Assert.Equal("Hello. World? Ok", CaseConverter.ToSentence("hello. world? ok"));

    [Theory]
    [InlineData("hello.  world", "Hello.  World")]
    [InlineData("hello!\tworld", "Hello!\tWorld")]
    [InlineData("hello?\r\nworld", "Hello?\r\nWorld")]
    [InlineData("hello.\n\n  world", "Hello.\n\n  World")]
    public void ToSentence_preserves_whitespace_between_sentences(string text, string expected)
        => Assert.Equal(expected, CaseConverter.ToSentence(text));
}
