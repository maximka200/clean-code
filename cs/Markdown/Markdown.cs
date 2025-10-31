using Markdown.Generator;
using Markdown.Lexer;
using Markdown.Parser;

namespace Markdown;

public class Markdown(MdLexer lexer, TokenParser parser, HtmlGenerator generator)
{
    public string ConvertToHtml(string text)
    {
        var tokens = lexer.Tokenize(text);
        var parseTree = parser.Parse(tokens);
        var html = generator.Generate(parseTree);
        
        return html;
    }
}