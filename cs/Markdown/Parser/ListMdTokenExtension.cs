using Markdown.Domain;
using Markdown.Domains.NodeExtentions;

namespace Markdown.Parser;

public static class ListMdTokenExtension
{
    internal static int HasSpaceBefore(this List<MdToken> tokens, int index)
    {
        var spaceCount = 0;
        while (tokens[index - 1].Type == TokenType.Space)
        {
            spaceCount++;
            index--;
        }
        return spaceCount;
    }
    
    
    internal static bool ContainsNumsOrSpaceOrUnderscore(this List<MdToken> tokens)
    {
        return tokens.Any(token => token.Type is TokenType.Number or TokenType.Space or TokenType.Underscore);
    }

    
    internal static void AddSymbol(this List<Node> root, string symbol, int count)
    {
        for (var _ = 0; _ < count; _++)
            root.Add(new TextNode(symbol));
    }

    internal static int FindClosing(this List<MdToken> tokens, int startIndex, string pattern)
    {
        for (var j = startIndex; j < tokens.Count - pattern.Length + 1; j++)
        {
            var match = true;
            for (var k = 0; k < pattern.Length; k++)
            {
                if (tokens[j + k].Type == TokenType.Underscore) continue;
                match = false;
                break;
            }

            if (match)
                return j;
        }
        return -1;
    }

    internal static int GetLengthChainOfTokenType(this List<MdToken> tokens, ref int startIndex, TokenType tokenType)
    {
        var tokenChainLength = 0;
        while (startIndex < tokens.Count && tokens[startIndex].Type == tokenType)
        {
            tokenChainLength++;
            startIndex++;
        }
        return tokenChainLength;
    }
}