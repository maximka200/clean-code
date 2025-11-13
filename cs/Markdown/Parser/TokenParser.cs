using Markdown.Domains;
using Markdown.Domains.Nodes;

namespace Markdown.Parser;

/// <summary>
///     Парсит лист токенов в синтаксическое дерево
/// </summary>
public class TokenParser(List<MdToken> tokens, int position = 0)
{
    private int position = position;
    
    public Node Parse(NodeContext context = NodeContext.None)
    {
        var rootChildren = new List<Node>();

        while (position < tokens.Count)
        {
            var token = tokens[position];

            switch (token.Type)
            {
                case TokenType.Grid:
                    HandleHeader(rootChildren);
                    continue;

                case TokenType.Escape:
                    HandleNewLine(rootChildren);
                    continue;

                case TokenType.Slash:
                    HandleSlashCharacter(rootChildren);
                    continue;

                case TokenType.Underscore:
                    HandleUnderscore(rootChildren, context);
                    continue;

                case TokenType.LeftSquareBracket:
                    HandleLeftSquareBracket(rootChildren, context);
                    continue;

                case TokenType.LeftParenthesis:
                case TokenType.RightParenthesis:
                case TokenType.Asterisk:
                case TokenType.Word:
                case TokenType.Number:
                case TokenType.Space:
                case TokenType.RightSquareBracket:
                case TokenType.Tab:
                default:
                    HandleText(token, rootChildren);
                    continue;
            }
        }

        return new Node(NodeType.Root, rootChildren);
    }
    
    private static TokenParser InitializeWith(List<MdToken> inputTokens)
    {
        var newTokenParser = new TokenParser(inputTokens);

        return newTokenParser;
    }

    private void HandleHeader(List<Node> rootChildren)
    {
        var level = GetCountHeaderLevel();

        if (position < tokens.Count && TokenIsTokenType(position, TokenType.Space) 
                                    && level <= HeaderNode.MaxHeaderLevel)
            ParseHeaderContent(level, rootChildren);
        else
            rootChildren.Add(new TextNode(new string('#', level)));
    }

    private void HandleNewLine(List<Node> rootChildren)
    {
        rootChildren.Add(new Node(NodeType.NewLine));
        Move();
    }

    private void HandleSlashCharacter(List<Node> rootChildren)
    {
        if (position + 1 < tokens.Count)
        {
            var next = tokens[position + 1];
            rootChildren.Add(new TextNode(next.Value));
            Move(2);
        }
        else
        {
            rootChildren.Add(new TextNode("\\"));
            Move();
        }
    }

    private void HandleUnderscore(List<Node> rootChildren, NodeContext context)
    {
        var underscoreCount = tokens.GetTokensCountAfter(position, TokenType.Underscore);
        Move(underscoreCount);
        var spaceCountAfter = tokens.GetTokensCountAfter(position, TokenType.Space);
        Move(spaceCountAfter);

        if (spaceCountAfter != 0)
        {
            rootChildren.AddSymbol("_", underscoreCount);
            rootChildren.AddSymbol(" ", spaceCountAfter);
            return;
        }
        
        if (underscoreCount is 1 or 2)
        {
            position += HandleUnderscoresAndReturnShift(underscoreCount, rootChildren, context);
        }
        else
        {
            rootChildren.AddSymbol("_", underscoreCount);
        }
    }

    private void HandleText(MdToken token, List<Node> rootChildren)
    {
        rootChildren.Add(new TextNode(token.Value));
        Move();
    }

    private void HandleLeftSquareBracket(List<Node> rootChildren, NodeContext context)
    {
        var linkNode = TryParseLink(context);

        if (linkNode != null)
        {
            rootChildren.Add(linkNode);
        }
        else
        {
            rootChildren.AddSymbol("[", 1);
            Move();
        }
    }

    private LinkNode? TryParseLink(NodeContext context)
    {
        var bracketsLength = tokens.GetTokensCountAfter(position, TokenType.LeftSquareBracket);
        var closeIndexMeaningText = tokens.FindClosing(position, bracketsLength, TokenType.RightSquareBracket);

        if (closeIndexMeaningText == -1 || TokenIsTokenType(closeIndexMeaningText - 1, TokenType.Slash)
            || !IsValidLinkSyntax(closeIndexMeaningText))
            return null;

        var closeIndexLinkText = tokens.FindClosing(closeIndexMeaningText + 2, 1, TokenType.RightParenthesis);
        if (closeIndexLinkText == -1 || TokenIsTokenType(closeIndexLinkText - 1, TokenType.Slash))
            return null;

        var linkNode = BuildLinkNode(
            meaningStart: position + 1,
            meaningEnd: closeIndexMeaningText,
            linkStart: closeIndexMeaningText + 2,
            linkEnd: closeIndexLinkText,
            context: context
        );

        Move(closeIndexLinkText + 1 - position);

        return linkNode;
    }
    
    private int HandleNonFormattingUnderscore(int closeIndex, int underscoreCount, int spaceCountBefore,
        List<Node> rootChildren, NodeContext context = NodeContext.None)
    {
        var tokensInside = tokens.GetRange(position, closeIndex - position - spaceCountBefore);
        var innerBlock = InitializeWith(tokensInside)
            .Parse(context).Children;
        rootChildren.AddSymbol("_", underscoreCount);
        rootChildren.AddRange(innerBlock);

        if (spaceCountBefore > 0)
            rootChildren.AddSymbol(" ", spaceCountBefore);

        rootChildren.AddSymbol("_", underscoreCount);
        return closeIndex - position + underscoreCount;
    }

    private int HandleUnderscoresAndReturnShift(int underscoreCount, List<Node> rootChildren,
        NodeContext context = NodeContext.None)
    {
        var closeIndex = tokens.FindClosing(position, underscoreCount, TokenType.Underscore);

        if (closeIndex == -1 || TokenIsTokenType(closeIndex - 1, TokenType.Slash)) 
        {
            rootChildren.AddSymbol("_", underscoreCount);
            return 0;
        }

        var spaceCountBefore = tokens.GetTokensCountBefore(closeIndex, TokenType.Space);

        if (spaceCountBefore > 0)
            return HandleNonFormattingUnderscore(closeIndex, underscoreCount, spaceCountBefore, rootChildren);

        if (ShouldHandleAsNonFormattingUnderscore(context, closeIndex, underscoreCount))
            return HandleNonFormattingUnderscore(closeIndex, underscoreCount, 0, rootChildren, context);

        var shift = CreateFormattingNode(underscoreCount, closeIndex, rootChildren);
        return shift;
    }
    
    private LinkNode BuildLinkNode(int meaningStart, int meaningEnd, int linkStart, int linkEnd, NodeContext context)
    {
        var meaningTokens = tokens.GetRange(meaningStart, meaningEnd - meaningStart);
        var linkTokens = tokens.GetRange(linkStart, linkEnd - linkStart);
        
        var meaningText = InitializeWith(meaningTokens).Parse(context).Children;
        var linkText = InitializeWith(linkTokens).Parse(context).Children;
        
        var linkNode = new LinkNode(LinkNodeType.LinkRoot, [
            new LinkNode(LinkNodeType.MeaningText, meaningText),
            new LinkNode(LinkNodeType.LinkText, linkText)
        ]);

        return linkNode;
    }
    
    private int CreateFormattingNode(int underscoreCount, int closeIndex, List<Node> rootChildren)
    {
        var innerTokens = tokens.GetRange(position, closeIndex - position);
        var nodeType = underscoreCount == 1 ? NodeType.Italic : NodeType.Bold;

        var parsedInnerBlock = InitializeWith(innerTokens)
            .Parse(Node.GetNodeContext(nodeType)).Children;

        rootChildren.Add(new Node(nodeType, parsedInnerBlock));

        var shift = closeIndex - position + underscoreCount;
        return shift;
    }
    
    private int GetCountHeaderLevel()
    {
        var level = 0;

        while (position < tokens.Count && tokens[position].Type == TokenType.Grid)
        {
            level++;
            Move();
        }

        return level;
    }

    private void ParseHeaderContent(int level, List<Node> rootChildren)
    {
        var spaceCount = tokens.GetTokensCountAfter(position, TokenType.Space);
        Move(spaceCount);

        var contentNodes = new List<Node>();
        while (position < tokens.Count && tokens[position].Type != TokenType.Escape)
        {
            contentNodes.Add(new TextNode(tokens[position].Value));
            Move();
        }

        rootChildren.Add(new HeaderNode(level, contentNodes));
    }

    private void Move(int shift = 1)
    {
        position += shift;
    }

    private bool ShouldHandleAsNonFormattingUnderscore(NodeContext context, int closeIndex,
        int underscoreCount)
    {
        return context == NodeContext.Italic
               || tokens.IsUnderscoreInDifferentWord(position - 1, closeIndex, underscoreCount)
               || tokens.IsUnderscoreInWordWithNumbers(position - 1, closeIndex, underscoreCount)
               || tokens.GetRange(position, closeIndex - position).HaveNotPairedUnderscore();
    }
    
    private bool IsValidLinkSyntax(int closeIndexMeaningText)
    {
        return closeIndexMeaningText < tokens.Count - 1 
               && tokens[closeIndexMeaningText + 1].Type == TokenType.LeftParenthesis 
               && HasPlaceForLinkText(closeIndexMeaningText);
    }
    
    private bool HasPlaceForLinkText(int closeIndexMeaningText)
    {
        return closeIndexMeaningText + 2 < tokens.Count;
    }

    private bool TokenIsTokenType(int index, TokenType type)
    {
        return tokens[index].Type == type;
    }
}