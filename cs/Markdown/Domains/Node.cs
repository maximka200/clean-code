using System.Text;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace Markdown.Domains;

public class Node(NodeType type, List<Node>? children = null) : IToHtml
{
    private NodeType Type { get; } = type;
    public List<Node> Children { get; } = children ?? [];

    public virtual void ToHtml(StringBuilder sb)
    {
        switch (Type)
        {
            case NodeType.Root:
                RenderRoot(sb);
                break;
            case NodeType.Italic:
                RenderItalic(sb);
                break;
            case NodeType.Bold:
                RenderBold(sb);
                break;
            case NodeType.NewLine:
                RenderNewLine(sb);
                break;
            case NodeType.Text:
            case NodeType.Header:
            case NodeType.Link:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    internal static NodeContext GetNodeContext(NodeType nodeType)
    {
        return nodeType switch
        {
            NodeType.Italic => NodeContext.Italic,
            _ => NodeContext.None
        };
    }

    private void RenderRoot(StringBuilder sb)
    {
        foreach (var child in Children)
            child.ToHtml(sb);
    }

    private void RenderItalic(StringBuilder sb)
    {
        sb.Append("<em>");
        foreach (var child in Children)
            child.ToHtml(sb);
        sb.Append("</em>");
    }

    private void RenderBold(StringBuilder sb)
    {
        sb.Append("<strong>");
        foreach (var child in Children)
            child.ToHtml(sb);
        sb.Append("</strong>");
    }

    private static void RenderNewLine(StringBuilder sb)
    {
        sb.Append("<br/>");
    }
}