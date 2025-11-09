using Markdown.Domains;
using Markdown.Domains.NodeExtensions;

namespace Markdown.Generator;

/// <summary>
/// Генерирует html текст из Node
/// </summary>
public static class HtmlGenerator
{
    public static string Generate(Node node)
    {
        return node.Type switch
        {
            NodeType.Root => string.Concat(node.Children.Select(Generate)),
            NodeType.Italic => $"<em>{string.Concat(node.Children.Select(Generate))}</em>",
            NodeType.Bold => $"<strong>{string.Concat(node.Children.Select(Generate))}</strong>",
            NodeType.NewLine => "<br/>",
            NodeType.Text => ((TextNode)node).Text,
            NodeType.Header => ToHeaderHtml((HeaderNode)node),
            _ => string.Concat(node.Children.Select(Generate))
        };
    }

    private static string ToHeaderHtml(HeaderNode node)
    {
        var level = node.Level;
        var inner = string.Concat(node.Children.Select(Generate));
        return $"<h{level}>{inner}</h{level}>";
    }
}