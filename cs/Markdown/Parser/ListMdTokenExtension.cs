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

        return hasNumber && (prevIsWord);
    }

    
    internal static void AddSymbol(this List<Node> root, string symbol, int count)
    {
        for (var _ = 0; _ < count; _++)
            root.Add(new TextNode(symbol));
    }

    internal static int FindClosing(this List<MdToken> tokens, int startIndex, string pattern, TokenType tokenType)
    {
        if (tokens is null) throw new ArgumentNullException(nameof(tokens));
        if (startIndex < 0 || startIndex >= tokens.Count) return -1;

        var patternLen = pattern?.Length ?? 0;
        if (patternLen <= 0) return -1;
        
        var maxStart = tokens.Count - patternLen;

        for (var j = startIndex; j <= maxStart; j++)
        {
            var match = true;
            for (var k = 0; k < patternLen; k++)
            {
                if (tokens[j + k].Type != tokenType)
                {
                    match = false;
                    break;
                }
            }

            if (!match)
                continue;
            
            var prevIsSame = (j - 1) >= 0 && tokens[j - 1].Type == tokenType;
            var nextIsSame = (j + patternLen) < tokens.Count && tokens[j + patternLen].Type == tokenType;

            if (prevIsSame || nextIsSame)
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

    internal static bool HaveNotPairedUnderscore(this List<MdToken> tokens)
    {
        return tokens.Count(token => token.Type == TokenType.Underscore) % 2 == 1;
    }
}