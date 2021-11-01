using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Web
{
    public static class DateTimeExtensions
    {
        private static CultureInfo ci = new CultureInfo("tr-TR");

        public static string ToFriendlyDateTimeString(this DateTime Date)
        {
            return FriendlyDate(Date) + " @ " + Date.ToString("t", ci).ToLower();
        }

        public static string ToFriendlyShortDateString(this DateTime Date)
        {
            return $"{Date.ToString("MMM dd", ci)}, {Date.Year}";
        }

        public static string ToFriendlyDateString(this DateTime Date)
        {
            return FriendlyDate(Date);
        }

        static string FriendlyDate(DateTime date)
        {
            string FormattedDate = "";
            if (date.Date == DateTime.Today)
            {
                FormattedDate = "Bugün";
            }
            else if (date.Date == DateTime.Today.AddDays(-1))
            {
                FormattedDate = "Dün";
            }
            else if (date.Date > DateTime.Today.AddDays(-6))
            {
                // *** Show the Day of the week
                FormattedDate = date.ToString("dddd").ToString();
            }
            else
            {
                FormattedDate = date.ToString("MMMM dd, yyyy");
            }
            return FormattedDate;
        }
    }
}
