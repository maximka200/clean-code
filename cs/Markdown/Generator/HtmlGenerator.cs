using System.Text;
using Markdown.Domains;

namespace Markdown.Generator;

/// <summary>
///     Генерирует html текст из Node
/// </summary>
public static class HtmlGenerator
{
    public static string Generate(IHtmlConverter node)
    {
        var sb = new StringBuilder();
        node.ConvertToHtml(sb);
        return sb.ToString();
    }
}