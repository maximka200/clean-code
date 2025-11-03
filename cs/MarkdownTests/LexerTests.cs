using FluentAssertions;
using Markdown.Domain;
using Markdown.Lexer;

namespace MarkdownTest;

public class Tests
{
    public static IEnumerable<TestCaseData> LexerTokenizeTestCases()
    {
        yield return new TestCaseData(
            "#Test",
            new List<MdToken>
            {
                new MdToken(TokenType.Grid, "#"),
                new MdToken(TokenType.Word, "Test")
            }
        ).SetName("WithGrid_1");
        
        yield return new TestCaseData(
            "_Test",
            new List<MdToken>
            {
                new MdToken(TokenType.Underscore, "_"),
                new MdToken(TokenType.Word, "Test")
            }
        ).SetName("WithUnderscore_1");
    }
    
    [Test]
    [TestCaseSource(nameof(LexerTokenizeTestCases))]
    public void Tokenize_ActualOutputShouldBeEqualExpectedOutput(string input, List<MdToken> expectedOutput)
    {
        var actualInput = MdLexer.Tokenize(input);
        
        actualInput.Should().BeEquivalentTo(expectedOutput);
    }
    
    [Test]
    [TestCase('T', TokenType.Letter)]
    [TestCase('#', TokenType.Grid)]
    [TestCase('_', TokenType.Underscore)]
    public void GetTokenType_ActualOutputShouldBeEqualExpectedOutput(char input, TokenType expectedOutput)
    {
        var actualInput = MdLexer.GetTokenType(input);
        
        actualInput.Should().Be(expectedOutput);
    }
}