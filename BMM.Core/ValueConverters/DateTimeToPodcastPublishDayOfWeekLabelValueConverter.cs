using System;
using System.Globalization;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class DateTimeToPodcastPublishDayOfWeekLabelValueConverter : MvxValueConverter<DateTime?, string>
    {
        protected override string Convert(DateTime? dateTime, Type targetType, object parameter, CultureInfo culture)
        {
            if (dateTime == null)
                return string.Empty;
            
            return dateTime
                .Value
                .ToNorwegianTime()
                .ToString(DateTimeConstants.FullDayFormat, CultureInfo.CurrentUICulture);
        }
    }
}