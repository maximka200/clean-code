using System.Text;

namespace Markdown.Domains;

public interface IToHtml
{
     void ToHtml(StringBuilder sb);
}