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
    
    // fix: добавить нормальную проверку на слово с числом, на _ внутри двух разных слов
    internal static bool ContainsTokenType(this List<MdToken> tokens, TokenType tokenType)
    {
        return tokens.Any(token => token.Type == tokenType);
    }

    // проверка на то, что под__черкивания в раз__ных словах, _разных сл_овах, раз_ных словах_
    public static bool IsUnderscoreInDifferentWord(this List<MdToken> tokens, int startIndex, int closeIndex, int underscoreCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(closeIndex, startIndex);

        var nextTokenInd = closeIndex + underscoreCount;
        if (nextTokenInd > tokens.Count)
            return false;

        var range = tokens.GetRange(startIndex, closeIndex - startIndex);
        var hasSeparator = range.ContainsTokenType(TokenType.Space) || range.ContainsTokenType(TokenType.Escape);

        var prevIsWord = startIndex > 0 && tokens[startIndex - 1].Type == TokenType.Word;
        var nextIsWord = nextTokenInd  < tokens.Count && tokens[nextTokenInd].Type == TokenType.Word;

        return hasSeparator && (prevIsWord || nextIsWord);
    }
    
    public static bool IsUnderscoreInWordWithNumbers(this List<MdToken> tokens, int startIndex, int closeIndex, int underscoreCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(closeIndex, startIndex);
        
        var range = tokens.GetRange(startIndex + underscoreCount, closeIndex - startIndex );

        var hasNumber = range.ContainsTokenType(TokenType.Number);
        
        var prevIsWord = startIndex > 0 && tokens[startIndex - 1].Type == TokenType.Word;
        var nextIsWord = closeIndex + underscoreCount < tokens.Count &&
                         tokens[closeIndex + underscoreCount].Type == TokenType.Word;

        return hasNumber && (prevIsWord || nextIsWord);
    }

    
    internal static void AddSymbol(this List<Node> root, string symbol, int count)
    {
        for (var _ = 0; _ < count; _++)
            root.Add(new TextNode(symbol));
    }

    internal static int FindClosing(this List<MdToken> tokens, int startIndex, string pattern, TokenType tokenType)
    {
        for (var j = startIndex; j < tokens.Count - pattern.Length + 1; j++)
        {
            var match = true;
            for (var k = 0; k < pattern.Length; k++)
            {
                if (tokens[j + k].Type != tokenType)
                {
                    match = false;
                    break;
                }
            }

            if (!match)
                continue;

            // доп проверка на случай: _токен__токен___
            var nextIndex = j + pattern.Length;
            var nextIsSame = nextIndex < tokens.Count && tokens[nextIndex].Type == tokenType;
            if (nextIsSame)
                continue; 

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