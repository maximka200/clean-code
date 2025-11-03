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

            if (tokenType == TokenType.Letter)
            {
                var word = new StringBuilder();
                word.Append(symbol);
                
                var j = i + 1;
                while (j < text.Length && char.IsLetter(text[j]))
                {
                    word.Append(text[j]);
                    j++;
                }

                tokens.Add(new MdToken(TokenType.Word, word.ToString()));
                
                i = j - 1;
            }
            else
            {
                tokens.Add(new MdToken(tokenType, symbol.ToString()));
            }
        }

        return tokens;
    }

    public static TokenType GetTokenType(char text) =>
        text switch
        {
            _ when char.IsLetter(text) => TokenType.Letter,
            '#' => TokenType.Grid,
            '_' => TokenType.Underscore,
            ' ' => TokenType.Space,
            _ => throw new ArgumentException($"Unknown token type: {text}")
        };
}
