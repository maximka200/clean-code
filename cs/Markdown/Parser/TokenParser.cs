using Markdown.Domains;
using Markdown.Domains.NodeExtensions;

namespace Markdown.Parser;

/// <summary>
/// Парсит лист токенов в синтаксическое дерево 
/// </summary>
public static class TokenParser
{
    public static Node Parse(List<MdToken> tokens, NodeContext context = NodeContext.None)
    {
        var rootChildren = new List<Node>();
        var i = 0;

        while (i < tokens.Count)
        {
            var token = tokens[i];

            switch (token.Type)
            {
                case TokenType.Grid:
                    HandleHeader(tokens, ref i, rootChildren);
                    continue;

                case TokenType.Escape:
                    HandleNewLine(rootChildren, ref i);
                    continue;

                case TokenType.Slash:
                    HandleEscapedCharacter(tokens, ref i, rootChildren);
                    continue;

                case TokenType.Underscore:
                    HandleUnderscore(tokens, ref i, rootChildren, context);
                    continue;

                case TokenType.Asterisk:
                // TODO: handle asterisk
                case TokenType.Word:
                case TokenType.Number:
                case TokenType.Space:
                default:
                    HandleText(token, rootChildren, ref i);
                    continue;
            }
        }

        return new Node(NodeType.Root, rootChildren);
    }

    private static void HandleHeader(List<MdToken> tokens, ref int i, List<Node> rootChildren)
    {
        var level = CountHeaderLevel(tokens, ref i);

        if (i < tokens.Count && tokens[i].Type == TokenType.Space)
            ParseHeaderContent(tokens, ref i, level, rootChildren);
        else
            rootChildren.Add(new TextNode(new string('#', level)));
    }

    private static void HandleNewLine(List<Node> rootChildren, ref int i)
    {
        rootChildren.Add(new Node(NodeType.NewLine));
        i++;
    }

    private static void HandleEscapedCharacter(List<MdToken> tokens, ref int i, List<Node> rootChildren)
    {
        if (i + 1 < tokens.Count)
        {
            var next = tokens[i + 1];
            rootChildren.Add(new TextNode(next.Value));
            i += 2;
        }
        else
        {
            rootChildren.Add(new TextNode("\\"));
            i++;
        }
    }


    private static void HandleUnderscore(List<MdToken> tokens, ref int i, List<Node> rootChildren, NodeContext context)
    {
        var underscoreCount = tokens.GetLengthChainOfTokenType(ref i, TokenType.Underscore);
        var spaceCountAfter = tokens.GetLengthChainOfTokenType(ref i, TokenType.Space);

        if (spaceCountAfter != 0)
        {
            rootChildren.AddSymbol("_", underscoreCount);
            rootChildren.AddSymbol(" ", underscoreCount);
            return;
        }

        switch (underscoreCount)
        {
            case 1 or 2:
                i += tokens.HandleUnderscoresAndReturnShift(i, underscoreCount, rootChildren, context);
                break;
            default:
                rootChildren.AddSymbol("_", underscoreCount);
                break;
        }
    }

    private static void HandleText(MdToken token, List<Node> rootChildren, ref int i)
    {
        rootChildren.Add(new TextNode(token.Value));
        i++;
    }

    private static int HandleNonFormattingUnderscore(
        List<MdToken> tokens,
        int i,
        int closeIndex,
        int underscoreCount,
        int spaceCountBefore,
        List<Node> rootChildren,
        NodeContext context = NodeContext.None)
    {
        var inner = Parse(tokens.GetRange(i, closeIndex - i - spaceCountBefore), context);
        rootChildren.AddSymbol("_", underscoreCount);
        rootChildren.AddRange(inner.Children);

        if (spaceCountBefore > 0)
            rootChildren.AddSymbol(" ", spaceCountBefore);

        rootChildren.AddSymbol("_", underscoreCount);
        return closeIndex - i + underscoreCount;
    }

    private static int HandleUnderscoresAndReturnShift(
        this List<MdToken> tokens,
        int i,
        int underscoreCount,
        List<Node> rootChildren,
        NodeContext context = NodeContext.None
    )
    {
        var closeIndex = tokens.FindClosing(i, underscoreCount, TokenType.Underscore);

        if (closeIndex != -1)
        {
            var spaceCountBefore = tokens.HasSpaceBefore(closeIndex);

            // проверка на пробелы до
            if (spaceCountBefore > 0)
                return HandleNonFormattingUnderscore(tokens, i, closeIndex, underscoreCount, spaceCountBefore,
                    rootChildren);

            var nodeType = underscoreCount == 1 ? NodeType.Italic : NodeType.Bold;

            // проверка на _ в разных словах, _ в словах с числами
            if (context == NodeContext.Italic
                || tokens.IsUnderscoreInDifferentWord(i - 1, closeIndex, underscoreCount)
                || tokens.IsUnderscoreInWordWithNumbers(i - 1, closeIndex, underscoreCount)
                || tokens.GetRange(i, closeIndex - i).HaveNotPairedUnderscore())
                return HandleNonFormattingUnderscore(tokens, i, closeIndex, underscoreCount, 0, rootChildren,
                    context);

            var inner = Parse(tokens.GetRange(i, closeIndex - i), Node.GetNodeContext(nodeType));

            rootChildren.Add(new Node(nodeType, inner.Children));
            return closeIndex - i + underscoreCount;
        }

        rootChildren.AddSymbol("_", underscoreCount);
        return 0;
    }

    private static int CountHeaderLevel(List<MdToken> tokens, ref int i)
    {
        var level = 0;

        while (i < tokens.Count && tokens[i].Type == TokenType.Grid)
        {
            level++;
            i++;
        }

        return level;
    }

    private static void ParseHeaderContent(List<MdToken> tokens, ref int i, int level, List<Node> rootChildren)
    {
        i++; // скип пробела

        var contentNodes = new List<Node>();
        while (i < tokens.Count && tokens[i].Type != TokenType.Escape)
        {
            contentNodes.Add(new TextNode(tokens[i].Value));
            i++;
        }

        rootChildren.Add(new HeaderNode(level, contentNodes));
    }
}