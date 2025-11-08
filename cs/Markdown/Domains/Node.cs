namespace Markdown.Domains;

public class Node(NodeType type, List<Node>? children = null)
{
    public NodeType Type { get; init; } = type;
    public List<Node> Children { get; init; } = children ?? [];
    
    internal static NodeContext GetNodeContext(NodeType nodeType)
    {
        return nodeType switch
        {
            NodeType.Bold => NodeContext.Bold,
            NodeType.Italic => NodeContext.Italic,
            _ => NodeContext.None
        };
    }
}