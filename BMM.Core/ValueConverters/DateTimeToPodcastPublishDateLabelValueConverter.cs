using System;
using System.Globalization;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class DateTimeToPodcastPublishDateLabelValueConverter : MvxValueConverter<DateTime?, string>
    {
        protected override string Convert(DateTime? date, Type targetType, object parameter, CultureInfo culture)
        {
            if (date == null)
                return string.Empty;
            
            string dateFormat = culture.DateTimeFormat.LongDatePattern
                .Replace(DateTimeConstants.FullMonthFormat, DateTimeConstants.ShortMonthFormat)
                .Replace(DateTimeConstants.FullDayFormat, string.Empty)
                .Trim()
                .Trim(StringConstants.Comma.ToCharArray())
                .Trim();
            
            return date
                .Value
                .ToNorwegianTime()
                .ToString(dateFormat, CultureInfo.CurrentUICulture);
        }
    }
}