using System;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class CoverUrlToFallbackImageValueConverter : MvxValueConverter<string, string>
    {
        protected override string Convert(string url, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(url))
                return (string)parameter;

            return url;
        }
    }
}