using FluentAssertions;
using Markdown.Domains;
using Markdown.Domains.NodeExtensions;
using Markdown.Generator;

namespace MarkdownTest.HtmlGenerator;

public class HtmlGeneratorTests
{
    public static IEnumerable<TestCaseData> HtmlGeneratorTestCases()
    {
        yield return new TestCaseData(
            new Node(NodeType.Root,
                new List<Node>
                {
                    new(NodeType.Italic, new List<Node> { new TextNode("Test") }),
                    new(NodeType.NewLine),
                    new HeaderNode(2, new List<Node>
                    {
                        new TextNode("Header"),
                        new TextNode(" "),
                        new TextNode("##"),
                        new TextNode(" "),
                    }),
                    new(NodeType.NewLine),
                    new(NodeType.Bold, new List<Node> { new TextNode("word") }),
                    new TextNode(" "),
                    new(NodeType.Italic, new List<Node> { new TextNode("word") })
                }),
            "<em>Test</em><br/><h2>Header ## </h2><br/><strong>word</strong> <em>word</em>"
        );
    }

    [Test]
    [TestCaseSource(nameof(HtmlGeneratorTestCases))]
    public void Parse_ShouldParse_Correctly(Node node, string expectedText)
    {
        var result = Markdown.Generator.HtmlGenerator.Generate(node);

        result.Should().BeEquivalentTo(expectedText);
    }
}