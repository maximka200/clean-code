namespace Markdown.Domain;

public class Node(NodeType type, string? text, List<Node>? childrens = null)
{
    public NodeType Type { get; init; } = type;
    public string Text { get; init; } = text ?? string.Empty;
    public List<Node> Childrens { get; init; } = childrens ?? new List<Node>();
}