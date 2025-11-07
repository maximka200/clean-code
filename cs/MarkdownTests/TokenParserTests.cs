using System.Collections.Generic;
using FluentAssertions;
using Markdown.Domain;
using Markdown.Domains.NodeExtentions;
using Markdown.Parser;
using NUnit.Framework;

namespace Markdown.Tests;

[TestFixture]
public class TokenParserTests
{
    public static IEnumerable<TestCaseData> TokenParserCases_Default()
    {
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Grid, "#"),
                new(TokenType.Space, " "),
                new(TokenType.Word, "Test")
            },
            new Node(NodeType.Root,  new List<Node>
            {
                new HeaderNode(1,new List<Node>
                {
                    new TextNode("Test")
                })
            })
        ).SetName("HeaderNode_Level1");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Space, " "),
                new(TokenType.Word, "Test")
            },
            new Node(NodeType.Root, new List<Node>
            {
                new HeaderNode(2,new List<Node>
                {
                    new TextNode( "Test")
                })
            })
        ).SetName("HeaderNode_Level2");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Space, " "),
                new(TokenType.Word, "Test")
            },
            new Node(NodeType.Root,  new List<Node>
            {
                new HeaderNode(6,  new List<Node>
                {
                    new TextNode( "Test")
                })
            })
        ).SetName("HeaderNode_Level6");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "Test"),
                new(TokenType.Underscore, "_")
            },
            new Node(NodeType.Root,  new List<Node>
            {
                new(NodeType.Italic,  new List<Node>
                {
                    new TextNode( "Test")
                })
            })
        ).SetName("ItalicUnderscoreNode");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Number, "123"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
            },
            new Node(NodeType.Root,  new List<Node>
            {
                new(NodeType.Bold,  new List<Node>
                {
                    new TextNode( "123")
                })
            })
        ).SetName("BoldUnderscoreNode");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "Test"),
                new(TokenType.Underscore, "_")
            },
            new Node(NodeType.Root,  new List<Node>
            {
                new(NodeType.Italic,  new List<Node>
                {
                    new TextNode( "Test")
                })
            })
        ).SetName("ItalicUnderscoreNode");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Number, "123"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
            },
            new Node(NodeType.Root,  new List<Node>
            {
                new(NodeType.Bold,  new List<Node>
                {
                    new TextNode("123")
                })
            })
        ).SetName("BoldUnderscoreNode");
        
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "Test"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Escape, "\n"),
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Space, " "),
                new(TokenType.Word, "Header"),
                new(TokenType.Space, " "),
                new(TokenType.Grid, "#"),
                new(TokenType.Grid, "#"),
                new(TokenType.Escape, "\n")
            },
            new Node(NodeType.Root, new List<Node>
            {
                new(NodeType.Italic, new List<Node>
                {
                    new TextNode("Test")
                }),
                new(NodeType.NewLine),
                new HeaderNode(2, new List<Node>
                {
                    new TextNode("Header"),
                    new TextNode(" "),
                    new TextNode("##"),
                    new TextNode(" "),
                }),
                new(NodeType.NewLine)
            })
        ).SetName("WithDifferentTags");
    }

    public static IEnumerable<TestCaseData> TokenParserCases_Extreme()
    {
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "UnclosedItalic")
            },
            new Node(NodeType.Root, new List<Node>
            {
                new TextNode("_"),
                new TextNode("UnclosedItalic")
            })
        ).SetName("UnclosedItalic");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "__"),
                new(TokenType.Word, "UnclosedBold")
            },
            new Node(NodeType.Root, new List<Node>
            {
                new TextNode("__"),
                new TextNode("UnclosedBold")
            })
        ).SetName("UnclosedBold");
        
        // Любой символ можно экранировать, чтобы он не считался частью разметки.
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Slash, "\\"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "SlashUnderscore"),
                new(TokenType.Slash, "\\"),
                new(TokenType.Underscore, "_")
            },
            new Node(NodeType.Root, new List<Node>
            {
                new TextNode("\\_"),
                new TextNode("SlashUnderscore"),
                new TextNode("\\_")
            })
        ).SetName("SlashUnderscoreBold_BothSides");
        
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Slash, "\\"),
                new(TokenType.Grid, "#"),
                new(TokenType.Space, " "),
                new(TokenType.Underscore, "HEADER")
            },
            new Node(NodeType.Root, new List<Node>
            {
                new TextNode("\\# "),
                new TextNode(" "),
                new TextNode("HEADER"),
            })
        ).SetName("SlashBeforeGrid");
        
        
        // Если внутри подчерков пустая строка ____, то они остаются символами подчерка.
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
            },
            new Node(NodeType.Root, new List<Node>
            {
                new TextNode("_"),
                new TextNode("_"),
                new TextNode("_"),
                new TextNode("_"),
            })
        ).SetName("FourSlash");
        
        
        // __Непарные_ символы в рамках одного абзаца не считаются выделением.
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "Непарные"),
                new(TokenType.Underscore, "_"),
            },
            new Node(NodeType.Root, new List<Node>
            {
                new TextNode("_"),
                new TextNode("_"),
                new TextNode("Непарные"),
                new TextNode("_"),
            })
        ).SetName("Unpaired");
        
        // Внутри __двойного выделения _одинарное_ тоже__ работает
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "test"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "test"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "test"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
            },
            new Node(NodeType.Root, new List<Node>
            {
                new(NodeType.Bold, new List<Node>
                {
                    new TextNode("test"),
                    new(NodeType.Italic, new List<Node>
                    {
                        new TextNode("test")
                    })
                })
            })).SetName("WithinDoubleSelectionSingle");
        
        // Но не наоборот — внутри _одинарного __двойное__ не_ работает.
        yield return new TestCaseData(
            new List<MdToken>
            {
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "test"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "test"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Underscore, "_"),
                new(TokenType.Word, "test"),
                new(TokenType.Underscore, "_"),
            },
            new Node(NodeType.Root, new List<Node>
            {
                new(NodeType.Italic, new List<Node>
                {
                    new TextNode("_"),
                    new TextNode("test"),
                    new TextNode("_"),
                    new TextNode("_"),
                    new TextNode("test"),
                    new TextNode("_"),
                    new TextNode("_"),
                    new TextNode("test"),
                    new TextNode("_"),
                })
            })).SetName("WithinSingleSelectionDouble");
        
            // Подчерки внутри текста c цифрами_12_3 не считаются выделением и должны оставаться символами подчерка
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Word, "цифрами"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Number, "12"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Number, "3"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new TextNode("цифрами_12_3"),
                })
            ).SetName("UnderscoreInNumbers");
            
            // Однако выделять часть слова они могут: и в _нач_але, и в сер_еди_не, и в кон_це._
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "нач"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "але"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new(NodeType.Italic, new List<Node> 
                        {
                            new TextNode("нач")
                        }
                    ),
                    new TextNode("але")
                })
            ).SetName("UnderscoreInWord_Start");
            
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Word, "сер"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "eди"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "не"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new TextNode("сер"),
                    new(NodeType.Italic, new List<Node> 
                        {
                            new TextNode("eди")
                        }
                    ),
                    new TextNode("не")
                })
            ).SetName("UnderscoreInWord_Middle");
            
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Word, "кон"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "це"),
                    new(TokenType.Underscore, "_"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new TextNode("кон"),
                    new(NodeType.Italic, new List<Node> 
                        {
                            new TextNode("це")
                        }
                    )
                })
            ).SetName("UnderscoreInWord_Finish");
            
            // В то же время выделение в ра_зных сл_овах не работает
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Word, "ра"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "зных"),
                    new(TokenType.Space, " "),
                    new(TokenType.Word, "сло"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "вах"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new TextNode("ра_зных сл_овах"),
                })
            ).SetName("UnderscoreDifferentWord");
            
            // эти_ подчерки_ и эти _подчерки _ не считаются выделением, 
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Space, " "),
                    new(TokenType.Word, "подчерки"),
                    new(TokenType.Underscore, "_"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new TextNode("_"),
                    new TextNode(" "),
                    new TextNode("подчерки"),
                    new TextNode("_"),
                })
            ).SetName("SpaceAfterOpenUnderscore");
            
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "подчерки"),
                    new(TokenType.Space, " "),
                    new(TokenType.Underscore, "_"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new TextNode("_"),
                    new TextNode("подчерки"),
                    new TextNode(" "),
                    new TextNode("_"),
                })
            ).SetName("SpaceBeforeClosedUnderscore");
            
            // В случае __пересечения _двойных__ и одинарных_ подчерков ни один из них не считается выделением.
            yield return new TestCaseData(
                new List<MdToken>
                {
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "пересечения"),
                    new(TokenType.Space, " "),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Word, "двойных"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Underscore, "_"),
                    new(TokenType.Space, " "),
                    new(TokenType.Word, "и"),
                    new(TokenType.Space, " "),
                    new(TokenType.Word, "одинарных"),
                    new(TokenType.Underscore, "_"),
                },
                new Node(NodeType.Root, new List<Node>
                {
                    new TextNode("__пересечения _двойных__ и одинарных_"),
                })
            ).SetName("UnionDoubleAndOneUnderscore");
    }
    
    [Test]
    [TestCaseSource(nameof(TokenParserCases_Default))]
    [TestCaseSource(nameof(TokenParserCases_Extreme))]
    public void Parse_ShouldParse_Correctly(List<MdToken> tokens, Node expectedNode)
    {
        var result = TokenParser.Parse(tokens);

        result.Should().BeEquivalentTo(expectedNode);
    }
}
