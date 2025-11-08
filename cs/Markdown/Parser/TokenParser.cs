using Markdown.Domain;
using Markdown.Domains.NodeExtentions;

namespace Markdown.Parser
{
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
                    // Headers 
                    case TokenType.Grid:
                        var level = 0;
                        
                        while (i < tokens.Count && tokens[i].Type == TokenType.Grid)
                        {
                            level++;
                            i++;
                        }

                        // Header must have space after hashes
                        if (i < tokens.Count && tokens[i].Type == TokenType.Space)
                        {
                            i++; // skip space
                            var contentNodes = new List<Node>();
                            while (i < tokens.Count && tokens[i].Type != TokenType.Escape)
                            {
                                contentNodes.Add(new TextNode(tokens[i].Value));
                                i++;
                            }

                            rootChildren.Add(new HeaderNode(level, contentNodes));
                            continue;
                        }

                        // not a valid header — treat as text
                        rootChildren.Add(new TextNode(new string('#', level)));
                        continue;
                    // --- New line ---
                    case TokenType.Escape:
                        rootChildren.Add(new Node(NodeType.NewLine));
                        i++;
                        continue;
                    // --- Escaped characters ---
                    case TokenType.Slash:
                    {
                        if (i + 1 < tokens.Count)
                        {
                            var next = tokens[i + 1];
                            rootChildren.Add(new TextNode("\\" + next.Value));
                            i += 2;
                        }
                        else
                        {
                            rootChildren.Add(new TextNode("\\"));
                            i++;
                        }

                        continue;
                    }
                    // --- Bold / Italic ---
                    case TokenType.Underscore:
                    {
                        var underscoreCount = tokens.GetLengthChainOfTokenType(ref i, TokenType.Underscore);

                        // check on space after underscore
                        var spaceCountAfter = tokens.GetLengthChainOfTokenType(ref i, TokenType.Space);

                        if (spaceCountAfter != 0)
                        {
                            rootChildren.AddSymbol("_", underscoreCount);
                            rootChildren.AddSymbol(" ", underscoreCount);
                            continue;
                        }

                        switch (underscoreCount)
                        {
                            // italic or bold
                            case 1 or 2:
                                i += tokens.ParseUnderscoresAndReturnShift(i, underscoreCount, rootChildren, context);
                                continue;
                            default:
                                rootChildren.AddSymbol("_", underscoreCount);
                                continue;
                        }
                    }
                    // --- Default: text tokens ---
                    case TokenType.Word:
                    case TokenType.Number:
                    case TokenType.Space:
                        rootChildren.Add(new TextNode(token.Value));
                        i++;
                        continue;
                    case TokenType.Asterisk:
                        // TODO: Asterisk logic
                        continue;
                    default:
                        // fallback
                        rootChildren.Add(new TextNode(token.Value));
                        i++;
                        break;
                }
            }

            return new Node(NodeType.Root, rootChildren);
        }

        private static int ParseUnderscoresAndReturnShift(
            this List<MdToken> tokens,
            int i,
            int underscoreCount,
            List<Node> rootChildren,
            NodeContext context = NodeContext.None
        )
        {
            var closeIndex = tokens.FindClosing(i, new string('_', underscoreCount), TokenType.Underscore);
            
            if (closeIndex != -1)
            {
                var spaceCountBefore = tokens.HasSpaceBefore(closeIndex);

                // проверка на пробелы до
                if (spaceCountBefore > 0)
                    return HandleNonFormattingUnderscore(tokens, i, closeIndex, underscoreCount, spaceCountBefore, rootChildren);
                
                // проверка на _ в разных словах, _ в словах с числами
                if (tokens.IsUnderscoreInDifferentWord(i - 1, closeIndex, underscoreCount) || tokens.IsUnderscoreInWordWithNumbers(i - 1, closeIndex, underscoreCount)
                    || tokens.GetRange(i, closeIndex - i).ContainsTokenType(TokenType.Underscore))
                    return HandleNonFormattingUnderscore(tokens, i, closeIndex, underscoreCount, 0, rootChildren);
                
                var nodeType = underscoreCount == 1 ? NodeType.Italic : NodeType.Bold;
                var inner = Parse(tokens.GetRange(i, closeIndex - i), Node.GetNodeContext(nodeType));

                rootChildren.Add(new Node(nodeType, inner.Children));
                return closeIndex - i + underscoreCount;
            }

            rootChildren.AddSymbol("_", underscoreCount);
            return 0;
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
    }
}
