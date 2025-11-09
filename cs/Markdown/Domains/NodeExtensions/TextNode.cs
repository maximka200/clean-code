namespace Markdown.Domains.NodeExtensions;

public class TextNode(string text, List<Node>? children = null) : Node(NodeType.Text, children)
{
    public string Text { get; } = text;
}