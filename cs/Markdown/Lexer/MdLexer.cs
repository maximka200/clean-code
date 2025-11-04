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
                    i = nextIndex - 1;
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
        var word = new StringBuilder();
        word.Append(text[startIndex]);

        var i = startIndex + 1;
        while (i < text.Length && predicate(text[i]))
        {
            word.Append(text[i]);
            i++;
        }

        return (word.ToString(), i);
    }

    public static TokenType GetTokenType(char text) =>
        text switch
        {
            _ when char.IsLetter(text) => TokenType.Word,
            _ when char.IsNumber(text) => TokenType.Number,
            '#' => TokenType.Grid,
            '_' => TokenType.Underscore,
            ' ' => TokenType.Space,
            _ => throw new ArgumentException($"Unknown token type: {text}")
        };
}
