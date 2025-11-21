using System.Text;

namespace Markdown.Domains.Nodes;

public class TextNode(string text, List<Node>? children = null) : Node(NodeType.Text, children)
{
    private string Text { get; } = text;
    
    public override void ConvertToHtml(StringBuilder sb)
    {
        sb.Append(Text);
    }
}