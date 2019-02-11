using System;
using System.Linq;
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
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}