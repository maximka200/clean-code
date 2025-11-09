using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Markdown.Lexer;

namespace MarkdownTest;

[TestFixture]
public class PerformanceTests
{
    [Test]
    public void Markdown_Render_ShouldWorkFast()
    {
        const int scale = 10;
        var sw = new Stopwatch();
        var timeSpans = new List<TimeSpan>();

        for (var length = 10; length <= 1000000; length *= scale)
        {
            var markdown = GenerateRandomMarkdown(length);
            sw.Start();

            var tokens = MdLexer.Tokenize(markdown);
            var ast = Markdown.Parser.TokenParser.Parse(tokens);
            _ = Markdown.Generator.HtmlGenerator.Generate(ast);

            sw.Stop();
            timeSpans.Add(sw.Elapsed);
            sw.Reset();
        }
        var timeRatios = Enumerable.Range(0, timeSpans.Count - 2)
            .Select(i => (double)timeSpans[i + 1].Ticks / timeSpans[i].Ticks);
        
        timeRatios.Should()
            .OnlyContain(timeRatio => timeRatio < Math.Log2(scale) * scale);
    }

    private static string GenerateRandomMarkdown(int len)
    {
        var rand = new Random();
        var specElements = new[] { " ", "_", "__", "#", "\\", "\\n" };
        var elements = "ABCDEFGHIJKLMNOPQRSTUVWXY1234567890".Select(c => c.ToString()).Concat(specElements).ToList();

        var sb = new StringBuilder();
        for (var i = 0; i < len; i++)
            sb.Append(elements[rand.Next(elements.Count)]);

        return sb.ToString();
    }
}