using Markdown.Domains;
using Markdown.Domains.NodeTypes;

namespace Markdown.Parser;

public static class ListMdTokenExtension
{
    /// <summary>
    ///     Поиск закрывающего индекса закрывающего токена
    /// </summary>
    /// <param name="tokens">Список токенов для анализа.</param>
    /// <param name="startIndex">Индекс первого токена после конца открывающей цепочки токенов</param>
    /// <param name="patternLen">Длина цепочки токенов, соответствующих шаблону</param>
    /// <param name="tokenType">Тип токена, который ищется</param>
    /// <returns>
    ///     Индекс первого токена в закрывающей цепочке, либо -1, если закрывающая цепочка не найдена.
    ///     Метод может выбросить исключение, если входные данные некорректны:
    ///     - <see cref="ArgumentNullException"/>: если список токенов равен null.
    ///     - <see cref="ArgumentOutOfRangeException"/>: если startIndex выходит за пределы допустимого диапазона.
    /// </returns>
    public static int FindClosing(this List<MdToken> tokens, int startIndex, int patternLen, TokenType tokenType)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        if (startIndex < 0 || startIndex >= tokens.Count) return -1;

        if (patternLen <= 0) return -1;

        var maxStart = tokens.Count - patternLen;

        for (var j = startIndex; j <= maxStart; j++)
        {
            if (!CheckMatch(tokens, j, patternLen, tokenType))
                continue;

            var prevIsSame = j - 1 >= 0 && tokens[j - 1].Type == tokenType;
            var nextIsSame = j + patternLen < tokens.Count && tokens[j + patternLen].Type == tokenType;

            var isSurroundedBySameTokens = prevIsSame || nextIsSame;

            if (!isSurroundedBySameTokens)
                return j;
        }

        return -1;
    }

    /// <summary>
    ///     Проверка на то, что подчёркивания находятся в разных словах.
    /// </summary>
    /// <param name="tokens">Список токенов для анализа.</param>
    /// <param name="startIndex">Индекс первого токена в цепочке подчёркиваний.</param>
    /// <param name="closeIndex">Индекс последнего токена в цепочке подчёркиваний.</param>
    /// <param name="underscoreCount">Количество подчёркиваний в цепочке.</param>
    /// <returns>
    ///     Возвращает true, если подчёркивания находятся в разных словах, иначе false.
    ///     Метод может выбросить исключение, если входные данные некорректны:
    ///     - <see cref="ArgumentOutOfRangeException"/>: если closeIndex меньше startIndex.
    /// </returns>
    public static bool IsUnderscoreInDifferentWord(this List<MdToken> tokens, int startIndex, int closeIndex,
        int underscoreCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(closeIndex, startIndex);

        var nextTokenInd = closeIndex + underscoreCount;
        if (nextTokenInd > tokens.Count)
            return false;

        var range = tokens.GetRange(startIndex, closeIndex - startIndex);
        var hasSeparator = range.ContainsTokenType(TokenType.Space) || range.ContainsTokenType(TokenType.Escape);

        var prevIsWord = startIndex > 0 && tokens[startIndex - 1].Type == TokenType.Word;
        var nextIsWord = nextTokenInd < tokens.Count && tokens[nextTokenInd].Type == TokenType.Word;

        return hasSeparator && (prevIsWord || nextIsWord);
    }

    /// <summary>
    ///     Проверка на то, что подчёркивания находятся в разных словах.
    /// </summary>
    /// <param name="tokens">Список токенов для анализа.</param>
    /// <param name="startIndex">Индекс первого токена в цепочке подчёркиваний.</param>
    /// <param name="closeIndex">Индекс последнего токена в цепочке подчёркиваний.</param>
    /// <param name="underscoreCount">Количество подчёркиваний в цепочке.</param>
    /// <returns>
    ///     Возвращает true, если подчёркивания находятся в разных словах, иначе false.
    ///     Метод может выбросить исключение, если входные данные некорректны:
    ///     - <see cref="ArgumentOutOfRangeException"/>: если closeIndex меньше startIndex.
    /// </returns>
    public static bool IsUnderscoreInWordWithNumbers(this List<MdToken> tokens, int startIndex, int closeIndex,
        int underscoreCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(closeIndex, startIndex);

        var hasNumber = tokens.GetRange(startIndex + underscoreCount, closeIndex - startIndex).ContainsTokenType(TokenType.Number);
        var prevIsWord = startIndex > 0 && tokens[startIndex - 1].Type == TokenType.Word;

        return hasNumber && prevIsWord;
    }
    
    internal static int GetTokensCountBefore(this List<MdToken> tokens, int index, TokenType tokenType)
    {
        var tokenChainLength = 0;
        while (index - 1 >= 0 && tokens[index - 1].Type == tokenType)
        {
            tokenChainLength++;
            index--;
        }

        return tokenChainLength;
    }

    internal static void AddSymbol(this List<Node> root, string symbol, int count)
    {
        for (var i = 0; i < count; i++)
            root.Add(new TextNode(symbol));
    }

    internal static int GetTokensCountAfter(this List<MdToken> tokens, int startIndex, TokenType tokenType)
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
    
    private static bool ContainsTokenType(this List<MdToken> tokens, TokenType tokenType)
    {
        return tokens.Any(token => token.Type == tokenType);
    }
    
    private static bool CheckMatch(List<MdToken> tokens, int startIndex, int patternLen, TokenType tokenType)
    {
        for (var k = 0; k < patternLen; k++)
            if (tokens[startIndex + k].Type != tokenType)
                return false;

        return true;
    }
}