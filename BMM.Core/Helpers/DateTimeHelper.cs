using System;

namespace BMM.Core.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime ToNorwegianTime(this DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            TimeZoneInfo norwegianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
            double differenceFromUtc = norwegianTimeZone.IsDaylightSavingTime(date) ? 2 : 1;

            return date.AddHours(differenceFromUtc);
        }
    }
}