namespace Markdown.Domains.NodeExtensions;

public class LinkNode(LinkNodeType type, List<Node>? children = null) : Node(NodeType.Link, children)
{
    public LinkNodeType LinkNodeType { get; } = type;
}