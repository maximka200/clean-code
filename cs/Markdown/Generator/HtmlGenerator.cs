using System.Text;
using Markdown.Domains;

namespace Markdown.Generator;

/// <summary>
///     Генерирует html текст из Node
/// </summary>
public static class HtmlGenerator
{
    public static string Generate(IToHtml node)
    {
        var sb = new StringBuilder();
        node.ToHtml(sb);
        return sb.ToString();
    }
}