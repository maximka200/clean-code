using Markdown.Generator;
using Markdown.Lexer;
using Markdown.Parser;

namespace Markdown;

public class Markdown
{
    public string ConvertToHtml(string text)
    {
        var tokens = MdLexer.Tokenize(text);
        var parseTree = TokenParser.Parse(tokens);
        var html = HtmlGenerator.Generate(parseTree);
        
        return html;
    }
}