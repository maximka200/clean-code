using Markdown.Domain;

namespace Markdown.Domains.NodeExtentions;

public class TextNode : Node
{
    public string Text { get; init; }

    public TextNode(string text, List<Node>? childrens = null)
        : base(NodeType.Text, childrens)
    {
        Text = text;
    }
}