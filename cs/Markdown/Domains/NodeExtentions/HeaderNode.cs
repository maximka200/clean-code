namespace Markdown.Domain.NodeExtentions;

public class HeaderNode(NodeType type, string? text, List<Node>? childrens) : Node(type, text, childrens)
{
    public static int MaxHeaderLevel = 6;
    public int Level { get; init;  }
}