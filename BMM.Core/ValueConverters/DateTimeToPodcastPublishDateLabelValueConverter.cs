using System;
using System.Globalization;
using BMM.Core.Constants;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class DateTimeToPodcastPublishDateLabelValueConverter : MvxValueConverter<DateTime?, string>
    {
        protected override string Convert(DateTime? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            
            string dateFormat = culture.DateTimeFormat.LongDatePattern
                .Replace(DateTimeConstants.FullMonthFormat, DateTimeConstants.ShortMonthFormat)
                .Replace(DateTimeConstants.FullDayFormat, string.Empty)
                .Trim()
                .Trim(StringConstants.Comma.ToCharArray())
                .Trim();
            
            return value.Value.ToString(dateFormat, CultureInfo.CurrentUICulture);
        }
    }
}