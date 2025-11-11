using FluentAssertions;

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
    [TestCase("[text](example.com)", "<a href=\"example.com\">text</a>")]
    [TestCase("Before [text](example.com) after", "Before <a href=\"example.com\">text</a> after")]
    [TestCase("Nested __[bold](link.com)__", "Nested <strong><a href=\"link.com\">bold</a></strong>")]
    public void Should_ConvertMarkdownToHtml_Correctly(string markdown, string expectedHtml)
    {
        var html = Markdown.Markdown.Render(markdown);

        html.Should().Be(expectedHtml);
    }
}