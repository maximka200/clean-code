namespace Markdown.Domains.NodeExtensions;

public class HeaderNode(int level = 1, List<Node>? children = null) : Node(NodeType.Header, children)
{
    public static int MaxHeaderLevel = 6;

    public int Level { get; init; } = level;
}