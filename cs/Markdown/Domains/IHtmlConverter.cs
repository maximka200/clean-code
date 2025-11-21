using System.Text;

namespace Markdown.Domains;

public interface IHtmlConverter
{
     void ConvertToHtml(StringBuilder sb);
}