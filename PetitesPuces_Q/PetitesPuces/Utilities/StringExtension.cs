using System.Web;

namespace PetitesPuces.Utilities
{
    public static class StringExtension
    {
        public static string Tail(this string source, int tail_length)
        {
            if(tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
        public static HtmlString ToHtml(this string source)
        {
            return new HtmlString(source);
        }
    }
}