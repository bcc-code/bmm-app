using MvvmCross.Converters;
using System;

namespace BMM.Core.ValueConverters
{
    public class MillisecondsToTimeValueConverter : MvxValueConverter<long, string>
    {
        protected override string Convert(long value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(TimeSpan.FromMilliseconds(value).Hours > 0)
            {
                return TimeSpan.FromMilliseconds(value).ToString(@"hh\:mm\:ss");
            }

            return TimeSpan.FromMilliseconds(value).ToString(@"mm\:ss");
        }
    }
}