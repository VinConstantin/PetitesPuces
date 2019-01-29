using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PetitesPuces.Utilities
{
    public static class Formatter
    {
        public static HtmlString Money(decimal? value)
        {
            decimal prix = value.GetValueOrDefault();
            var valueStr = (prix).ToString("0.00");
            var splitVal = valueStr.Split(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);

            var str = "<div style='display:inline-block;width:100%;text-align: right;'><strong>"+splitVal[0]+"</strong>" +
                         "<sup>"+(splitVal.Length == 2 ? splitVal[1] : "00") +"</sup> " +
                         "<strong>$</strong></div>";
            
            HtmlString html = new HtmlString(str);
            return html;
        }
        public static HtmlString Money(decimal value)
        {
            var valueStr = (value).ToString("0.00");
            var splitVal = valueStr.Split(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);

            var str = "<div style='display:inline-block;width:100%;text-align: right;'><strong>"+splitVal[0]+"</strong>" +
                      "<sup>"+(splitVal.Length == 2 ? splitVal[1] : "00") +"</sup> " +
                      "<strong>$</strong></div>";
            
            HtmlString html = new HtmlString(str);
            return html;
        }
        public static HtmlString Money(double value)
        {
            var valueStr = (value).ToString("0.00");
            var splitVal = valueStr.Split(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);

            var str = "<div style='display:inline-block;width:100%;text-align: right;'><strong>" + splitVal[0] + "</strong>" +
                         "<sup>" + (splitVal.Length == 2 ? splitVal[1] : "00") + "</sup> " +
                         "<strong>$</strong></div>";

            HtmlString html = new HtmlString(str);
            return html;
        }
        public static Color GetNextColor()
        {        
            Random _random = new Random();
            return Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));
        }
    }
    
}