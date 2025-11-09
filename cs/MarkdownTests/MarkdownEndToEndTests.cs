using FluentAssertions;
using Markdown.Lexer;

namespace MarkdownTest;

[TestFixture]
public class MarkdownEndToEndTests
{
    [Test]
    [TestCase("_Test_\n## Header ##\n__word__ _word_",
        "<em>Test</em><br/><h2>Header ##</h2><br/><strong>word</strong> <em>word</em>")]
    [TestCase("Plain text", "Plain text")]
    [TestCase("# H1\n## H2\n### H3", "<h1>H1</h1><br/><h2>H2</h2><br/><h3>H3</h3>")]
    [TestCase("Text with _underscores_ inside", "Text with <em>underscores</em> inside")]
    [TestCase("__Bold__ _Italic_ __Mixed__", "<strong>Bold</strong> <em>Italic</em> <strong>Mixed</strong>")]
    [TestCase(@"Escaped \_underscore\_", "Escaped _underscore_")]
    public void Should_ConvertMarkdownToHtml_Correctly(string markdown, string expectedHtml)
    {
        var tokens = MdLexer.Tokenize(markdown);
        var ast = Markdown.Parser.TokenParser.Parse(tokens);
        var html = Markdown.Generator.HtmlGenerator.Generate(ast);

        html.Should().Be(expectedHtml);
    }
}