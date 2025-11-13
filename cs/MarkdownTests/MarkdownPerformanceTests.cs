using System.Diagnostics;
using System.Text;
using FluentAssertions;
using static Markdown.Markdown;
using TimeSpan = System.TimeSpan;

namespace MarkdownTest;

[TestFixture]
public class MarkdownPerformanceTests
{
    [Test]
    [Repeat(100)]
    public void Markdown_Render_ShouldWorkFastThanNLogN()
    {
        const int scale = 10;
        var sw = new Stopwatch();
        var timeSpans = new List<TimeSpan>();

        for (var length = 10; length <= 1000000; length *= scale)
        {
            var markdown = GenerateRandomMarkdown(length);
            sw.Start();
            GC.Collect();
            Render(markdown);

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
        var elements = Enumerable.Range(32, 96).Select(c => ((char)c).ToString()).ToList();
        var sb = new StringBuilder();
        for (var i = 0; i < len; i++)
            sb.Append(elements[Random.Shared.Next(elements.Count)]);

        return sb.ToString();
    }
}