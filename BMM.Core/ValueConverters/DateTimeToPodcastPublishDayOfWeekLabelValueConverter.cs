using System;
using System.Globalization;
using BMM.Core.Constants;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class DateTimeToPodcastPublishDayOfWeekLabelValueConverter : MvxValueConverter<DateTime?, string>
    {
        protected override string Convert(DateTime? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            
            return value.Value.ToString(DateTimeConstants.FullDayFormat, CultureInfo.CurrentUICulture);
        }
    }
}