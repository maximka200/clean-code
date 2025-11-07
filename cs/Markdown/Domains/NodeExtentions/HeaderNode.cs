using Markdown.Domain;

namespace Markdown.Domains.NodeExtentions;

public class HeaderNode : Node
{
    public static int MaxHeaderLevel = 6;

    public int Level { get; init; }

    public HeaderNode(int level = 1, List<Node>? childrens = null)
        : base(NodeType.Header, childrens)
    {
        Level = level;
    }
}