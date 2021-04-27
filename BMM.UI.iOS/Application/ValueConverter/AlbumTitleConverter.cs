using BMM.Api.Implementation.Models;
using MvvmCross.Converters;
using System;
using System.Globalization;

namespace BMM.UI.iOS
{
    public class AlbumTitleConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            else
                return ((Album)value).Title;
        }
    }
}