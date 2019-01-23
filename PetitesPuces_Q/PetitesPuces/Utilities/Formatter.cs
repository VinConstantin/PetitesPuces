using System;
using System.Web;

namespace PetitesPuces.Utilities
{
    public static class Formatter
    {
        public static HtmlString Money(double value)
        {
            var str = "<strong>"+value.ToString("0")+"</strong>" +
                         "<sup>"+value.ToString("1.00").Split('.')[1]+"</sup> " +
                         "<strong>$</strong>";
            
            HtmlString html = new HtmlString(str);
            return html;
        }
    }
}