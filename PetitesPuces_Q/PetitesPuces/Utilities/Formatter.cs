using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PetitesPuces.Utilities
{
    public static class Formatter
    {
        public static HtmlString Money(decimal? value, bool aDroite = true)
        {
            decimal prix = value.GetValueOrDefault();
            var valueStr = (prix).ToString("0.00");
            var splitVal = valueStr.Split(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);
            string style = aDroite ? "style='display:inline-block;width:100%;text-align: right;'":"";
            var str = "<span "+style+" ><strong>"+splitVal[0]+"</strong>" +
                         "<sup>"+(splitVal.Length == 2 ? splitVal[1] : "00") +"</sup> " +
                         "<strong>$</strong></span>";
            
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

            var str = "<div style='display:inline-block;width:100%;text-align: right;'><strong>"+splitVal[0]+"</strong>" +
                      "<sup>"+(splitVal.Length == 2 ? splitVal[1] : "00") +"</sup> " +
                      "<strong>$</strong></div>";
            
            HtmlString html = new HtmlString(str);
            return html;
        }
        public static HtmlString Poids(decimal? value)
        {
            decimal poids = value.GetValueOrDefault();
            var valueStr = (poids).ToString("0.00");

            var str = "<strong>"+valueStr+"</strong> lbs";
            
            HtmlString html = new HtmlString(str);
            return html;
        }
        public static Color GetNextColor()
        {        
            Random _random = new Random();
            return Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));
        }

        public static HtmlString Percentage(double value)
        {
            var valueStr = (value * 100).ToString("0");
            var splitVal = valueStr.Split(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]);

            var str = "<div style='display:inline-block;width:100%;text-align: right;'><strong>"+splitVal[0]+"</strong>" +
                      "<strong>%</strong></div>";
            
            HtmlString html = new HtmlString(str);
            return html;
        }
    }
    
}