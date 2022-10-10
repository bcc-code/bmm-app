using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.UI.iOS.Extensions;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class PinnedItemIconToImageConverter : MvxValueConverter<PinnedItem, string>
    {
        protected override string Convert(PinnedItem pinnedItem, Type targetType, object parameter, CultureInfo culture)
        {
            if (pinnedItem?.Icon != "")
                return $"res:{pinnedItem!.Icon.ToIosImageName()}";

            return "res:icon_category_unknown.png";
        }
    }
}