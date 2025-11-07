using System.Text;
using Markdown.Domain;

namespace Markdown.Lexer;

/// <summary>
/// Разбивает текст на Md токены
/// </summary>
public static class MdLexer
{
    public static List<MdToken> Tokenize(string text)
    {
        var tokens = new List<MdToken>();
    
        for (var i = 0; i < text.Length; i++)
        {
            var symbol = text[i];
            var tokenType = GetTokenType(symbol);

            switch (tokenType)
            {
                case TokenType.Word or TokenType.Number:
                    var (value, nextIndex) = CollectFullValue(text, i,
                        tokenType is TokenType.Word ? char.IsLetter : char.IsNumber
                        );
                    tokens.Add(new MdToken(tokenType, value));
                    i = nextIndex;
                    break;
                default:
                    tokens.Add(new MdToken(tokenType, symbol.ToString()));
                    break;
            }
        }

        return tokens;
    }

    private static (string word, int nextIndex) CollectFullValue(string text, int startIndex, Func<char, bool> predicate)
    {
        var value = new StringBuilder();
        value.Append(text[startIndex]);

        var i = startIndex + 1;
        while (i < text.Length && predicate(text[i]))
        {
            value.Append(text[i]);
            i++;
        }

        return (value.ToString(), i - 1);
    }

    public static TokenType GetTokenType(char text) =>
        text switch
        {
            _ when char.IsLetter(text) => TokenType.Word,
            _ when char.IsNumber(text) => TokenType.Number,
            '#' => TokenType.Grid,
            '*' => TokenType.Asterisk,
            '_' => TokenType.Underscore,
            ' ' or '\u00a0' or '\u200b' => TokenType.Space,
            '\n' or '\r' => TokenType.Escape,
            '\\' => TokenType.Slash,
            _ => throw new ArgumentOutOfRangeException($"Unknown token type: {text}")
        };
}
