using System;

namespace PetitesPuces.Utilities
{
    public static class DateTimeExtension
    {
        public static String ToShortDateString(this DateTime? dt)
        {
            if (dt == null) return "";
            
            DateTime date = (DateTime) dt;
            return date.ToShortDateString();

        }
        public static String ToShortTimeString(this DateTime? dt)
        {
            if (dt == null) return "";
            
            DateTime date = (DateTime) dt;
            return date.ToShortTimeString();
        }
    }
}