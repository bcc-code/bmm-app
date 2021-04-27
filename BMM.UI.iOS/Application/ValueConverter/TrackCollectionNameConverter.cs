using MvvmCross.Converters;
using System;
using System.Globalization;
using BMM.Api.Implementation.Models;

namespace BMM.UI.iOS
{
    public class TrackCollectionNameConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as TrackCollection)?.Name ?? "";
        }
    }
}