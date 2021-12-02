using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.UI.iOS.Extensions;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class PinnedItemIconToImageConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameters = (object[])parameter;
            var pinnedItem = (PinnedItem)((Func<Document>)parameters[0]).Invoke();

            if (pinnedItem?.Icon != "")
                return $"res:{pinnedItem!.Icon.ToIosImageName()}";

            return "res:icon_category_unknown.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}