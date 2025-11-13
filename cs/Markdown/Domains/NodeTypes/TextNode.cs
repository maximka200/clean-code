using System.Text;

namespace Markdown.Domains.NodeTypes;

public class TextNode(string text, List<Node>? children = null) : Node(NodeType.Text, children)
{
    private string Text { get; } = text;
    
    public override void ToHtml(StringBuilder sb)
    {
        sb.Append(Text);
    }
}