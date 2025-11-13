using System.Text;

namespace Markdown.Domains.NodeTypes;

public class HeaderNode : Node
{
    private const int MaxHeaderLevel = 6;

    private int Level { get; }

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
    
    public override void ToHtml(StringBuilder sb)
    {
        sb.Append($"<h{Level}>");
        foreach (var child in Children)
        {
            child.ToHtml(sb);
        }
        sb.Append($"</h{Level}>");
    }
}