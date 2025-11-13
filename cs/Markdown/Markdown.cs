using Markdown.Generator;
using Markdown.Lexer;
using Markdown.Parser;

namespace Markdown;

public static class Markdown
{
    public static string Render(string text)
    {
        var tokens = MdLexer.Tokenize(text);
        var parser = new TokenParser(tokens);
        var parseTree = parser.Parse();
        var html = HtmlGenerator.Generate(parseTree);

        return html;
    }
}
