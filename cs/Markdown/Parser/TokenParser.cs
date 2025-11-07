using System.Collections.Generic;
using System.Linq;
using Markdown.Domain;
using Markdown.Domains.NodeExtentions;

namespace Markdown.Parser
{
    public static class TokenParser
    {
        public static Node Parse(List<MdToken> tokens)
        {
            var rootChildren = new List<Node>();
            var i = 0;
            while (i < tokens.Count)
            {
                var token = tokens[i];

                // Headers 
                if (token.Type == TokenType.Grid)
                {
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
                }

                // --- New line ---
                if (token.Type == TokenType.Escape)
                {
                    rootChildren.Add(new Node(NodeType.NewLine));
                    i++;
                    continue;
                }

                // --- Escaped characters ---
                if (token.Type == TokenType.Slash)
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
                if (token.Type == TokenType.Underscore)
                {
                    var underscoreCount = 0;
                    while (i < tokens.Count && tokens[i].Type == TokenType.Underscore)
                    {
                        underscoreCount++;
                        i++;
                    }
                    
                    // check on space after underscore
                    var spaceCountAfter = 0;
                    while (i < tokens.Count && tokens[i].Type == TokenType.Space)
                    {
                        spaceCountAfter++;
                        i++;
                    }

                    if (spaceCountAfter != 0)
                    {
                        AddSymbol("_", underscoreCount, rootChildren);
                        AddSymbol(" ", underscoreCount, rootChildren);
                        continue;
                    }
                    
                    // italic
                    if (underscoreCount == 1)
                    {
                        var closeIndex = FindClosing(tokens, i, "_");
                        
                        if (closeIndex != -1)
                        {
                            var inner = Parse(tokens.GetRange(i, closeIndex - i));
                            rootChildren.Add(new Node(NodeType.Italic, inner.Children));
                            i = closeIndex + 1;
                            continue;
                        }
                        AddSymbol("_", underscoreCount, rootChildren);
                        continue;
                    }
                    
                    // bold
                    if (underscoreCount == 2)
                    {
                        var closeIndex = FindClosing(tokens, i, "__");
                        if (closeIndex != -1)
                        {
                            var inner = Parse(tokens.GetRange(i, closeIndex - i));
                            rootChildren.Add(new Node(NodeType.Bold, inner.Children));
                            i = closeIndex + 2;
                            continue;
                        }
                        AddSymbol("_", underscoreCount, rootChildren);
                        continue;
                    }
                    
                    AddSymbol("_", underscoreCount, rootChildren);
                    continue;
                }

                // --- Default: text tokens ---
                if (token.Type == TokenType.Word ||
                    token.Type == TokenType.Number ||
                    token.Type == TokenType.Space)
                {
                    rootChildren.Add(new TextNode(token.Value));
                    i++;
                    continue;
                }

                // fallback
                rootChildren.Add(new TextNode(token.Value));
                i++;
            }

            return new Node(NodeType.Root, rootChildren);
        }

        private static void AddSymbol(string symbol, int count, List<Node> root)
        {
            for (var _ = 0; _ < count; _++)
                root.Add(new TextNode(symbol));
        }

        private static int FindClosing(List<MdToken> tokens, int startIndex, string pattern)
        {
            for (var j = startIndex; j < tokens.Count - pattern.Length + 1; j++)
            {
                var match = true;
                for (var k = 0; k < pattern.Length; k++)
                {
                    if (tokens[j + k].Type != TokenType.Underscore)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return j;
            }
            return -1;
        }
    }
}
