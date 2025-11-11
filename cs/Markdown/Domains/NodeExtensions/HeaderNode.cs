namespace Markdown.Domains.NodeExtensions;

public class HeaderNode : Node
{
    private const int MaxHeaderLevel = 6;

    public int Level { get; }

    public HeaderNode(int level = 1, List<Node>? children = null)
        : base(NodeType.Header, children)
    {
        if (level is < 1 or > MaxHeaderLevel)
            throw new ArgumentOutOfRangeException(
                nameof(level),
                $"Header level must be between 1 and {MaxHeaderLevel}."
            );

        Level = level;
    }
}