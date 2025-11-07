namespace Markdown.Domain;

public class Node(NodeType type, List<Node>? children = null)
{
    public NodeType Type { get; init; } = type;
    public List<Node> Children { get; init; } = children ?? new List<Node>();
}