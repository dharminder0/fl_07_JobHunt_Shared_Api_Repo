using System;
using System.Collections.Generic;
using System.Text;

namespace VendersCloud.Common.Extensions {
    public static class DateTimeExtensions {
        public static DateTime ToAmsterdamTimeNow(this DateTime date) {
            TimeZoneInfo wEurope = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            DateTime utcTime = date.ToUniversalTime();
            var offSet = wEurope.GetUtcOffset(utcTime);
            return utcTime.AddHours(offSet.TotalHours);
        }

        public static double GetAmsterdamUTCTimeOffset(this DateTime date) {
            TimeZoneInfo wEurope = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            DateTime utcTime = date.ToUniversalTime();

            var offSet = wEurope.GetUtcOffset(utcTime);
            return offSet.TotalHours;
        }
    }
}
