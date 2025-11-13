using System.Text;

namespace Markdown.Domains.Nodes;

public class LinkNode(LinkNodeType type, List<Node>? children = null) 
    : Node(NodeType.Link, children)
{
    private LinkNodeType LinkNodeType { get; } = type;
    
    public override void ToHtml(StringBuilder sb)
    {
        if (LinkNodeType != LinkNodeType.LinkRoot || Children.Count < 2)
        {
            foreach (var child in Children)
                child.ToHtml(sb);
            return;
        }

        var textNode = Children
            .OfType<LinkNode>()
            .FirstOrDefault(n => n.LinkNodeType == LinkNodeType.MeaningText);

        var urlNode = Children
            .OfType<LinkNode>()
            .FirstOrDefault(n => n.LinkNodeType == LinkNodeType.LinkText);

        if (textNode == null || urlNode == null)
        {
            foreach (var child in Children)
                child.ToHtml(sb);
            return;
        }
        
        var textBuilder = new StringBuilder();
        foreach (var child in textNode.Children)
            child.ToHtml(textBuilder);

        var urlBuilder = new StringBuilder();
        foreach (var child in urlNode.Children)
            child.ToHtml(urlBuilder);
        
        sb.Append("<a href=\"");
        sb.Append(urlBuilder);
        sb.Append("\">");
        sb.Append(textBuilder);
        sb.Append("</a>");
    }
}