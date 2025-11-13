using FluentAssertions;
using static Markdown.Markdown;

namespace MarkdownTest;

[TestFixture]
public class MarkdownFuncTests
{
    [Test]
    [TestCase("Text with _underscores_ inside", "Text with <em>underscores</em> inside")]
    [TestCase("__Bold__ _Italic_ __Mixed__", "<strong>Bold</strong> <em>Italic</em> <strong>Mixed</strong>")]
    [TestCase("root1_2_3", "root1_2_3")]
    [TestCase("root1__2__3", "root1__2__3")]
    [TestCase("__text _text_ text__", "<strong>text <em>text</em> text</strong>")]
    [TestCase("_text __text__ text_", "<em>text __text__ text</em>")]
    [TestCase("____", "____")]
    public void Should_ConvertMarkdownWithBoldAndItalicToHtml_Correctly(string markdown, string expectedHtml)
    {
        var html = Render(markdown);

        html.Should().Be(expectedHtml);
    }
    
    [Test]
    [TestCase("# H1\n## H2\n### H3", "<h1>H1</h1><br/><h2>H2</h2><br/><h3>H3</h3>")]
    [TestCase("#### H4\n##### H5\n###### H6", "<h4>H4</h4><br/><h5>H5</h5><br/><h6>H6</h6>")]
    [TestCase("#  H1\n##  H2\n###  H3", "<h1>H1</h1><br/><h2>H2</h2><br/><h3>H3</h3>")]
    [TestCase("# _text_ __text__", "<h1><em>text</em> <strong>text</strong></h1>")]
    [TestCase("  ## h1", "  ## h1")]
    [TestCase("########### h1", "########### h1")]
    public void Should_ConvertMarkdownWithHeadersToHtml_Correctly(string markdown, string expectedHtml)
    {
        var html = Render(markdown);

        html.Should().Be(expectedHtml);
    }
    
    [Test]
    [TestCase(@"Escaped \_underscore\_", "Escaped _underscore_")]
    [TestCase(@"Escaped \_underscore_", "Escaped _underscore_")]
    [TestCase(@"Escaped _underscore\_", "Escaped _underscore_")]
    [TestCase(@"Escaped _underscore\\_", "Escaped _underscore\\_")]
    [TestCase(@"\## text", "## text")]
    [TestCase(@"\#\# text", "## text")]
    [TestCase(@"\[text](example.com)", "[text](example.com)")]
    [TestCase(@"[text\](example.com)", "[text](example.com)")]
    [TestCase(@"\[text]\(example.com)", "[text](example.com)")]
    [TestCase(@"[text](example.com\)", "[text](example.com)")]
    [TestCase(@"\\[text](example.com)", "\\<a href=\"example.com\">text</a>")]
    public void Should_ConvertMarkdownWithShieldingToHtml_Correctly(string markdown, string expectedHtml)
    {
        var html = Render(markdown);

        html.Should().Be(expectedHtml);
    }
    
    [Test]
    [TestCase("[text](example.com)", "<a href=\"example.com\">text</a>")]
    [TestCase("Before [text](example.com) after", "Before <a href=\"example.com\">text</a> after")]
    [TestCase("[# text](example.com)", "<a href=\"example.com\"><h1>text</h1></a>")]
    [TestCase("[# __text__](example.com)", "<a href=\"example.com\"><h1><strong>text</strong></h1></a>")]
    [TestCase("Nested __[bold](link.com)__", "Nested <strong><a href=\"link.com\">bold</a></strong>")]
    public void Should_ConvertMarkdownWithLinkToHtml_Correctly(string markdown, string expectedHtml)
    {
        var html = Render(markdown);

        html.Should().Be(expectedHtml);
    }
    
    [Test]
    [TestCase("TEXT!???", "TEXT!???")]
    [TestCase("_Test_\n## Header ##\n__word__ _word_",
        "<em>Test</em><br/><h2>Header ##</h2><br/><strong>word</strong> <em>word</em>")]
    public void Should_ConvertMarkdownGeneralToHtml_Correctly(string markdown, string expectedHtml)
    {
        var html = Render(markdown);

        html.Should().Be(expectedHtml);
    }
}