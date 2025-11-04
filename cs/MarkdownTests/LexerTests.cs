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
        ).SetName("WithGridAndWord_1");
        
        yield return new TestCaseData(
            "_Test",
            new List<MdToken>
            {
                new MdToken(TokenType.Underscore, "_"),
                new MdToken(TokenType.Word, "Test")
            }
        ).SetName("WithUnderscoreAndWord_1");
        
        yield return new TestCaseData(
            "_123",
            new List<MdToken>
            {
                new MdToken(TokenType.Underscore, "_"),
                new MdToken(TokenType.Number, "123")
            }
        ).SetName("WithUnderscoreAndNumber_1");
        
        yield return new TestCaseData(
            "123Test",
            new List<MdToken>
            {
                new MdToken(TokenType.Number, "123"),
                new MdToken(TokenType.Word, "Test")
            }
        ).SetName("WithWordAndNumber_1");
        
        yield return new TestCaseData(
            "_Test_ #Text 123",
            new List<MdToken>
            {
                new MdToken(TokenType.Underscore, "_"),
                new MdToken(TokenType.Word, "Test"),
                new MdToken(TokenType.Underscore, "_"),
                new MdToken(TokenType.Space, " "),
                new MdToken(TokenType.Grid, "#"),
                new MdToken(TokenType.Word, "Text"),
                new MdToken(TokenType.Space, " "),
                new MdToken(TokenType.Number, "123")
            }
        ).SetName("WithAllTokens_1");
    }
    
    [Test]
    [TestCaseSource(nameof(LexerTokenizeTestCases))]
    public void Tokenize_ActualOutputShouldBeEqualExpectedOutput(string input, List<MdToken> expectedOutput)
    {
        var actualInput = MdLexer.Tokenize(input);
        
        actualInput.Should().BeEquivalentTo(expectedOutput);
    }
    
    [Test]
    [TestCase('T', TokenType.Word)]
    [TestCase('#', TokenType.Grid)]
    [TestCase('_', TokenType.Underscore)]
    [TestCase('1', TokenType.Number)]
    public void GetTokenType_ActualOutputShouldBeEqualExpectedOutput(char input, TokenType expectedOutput)
    {
        var actualInput = MdLexer.GetTokenType(input);
        
        actualInput.Should().Be(expectedOutput);
    }
}