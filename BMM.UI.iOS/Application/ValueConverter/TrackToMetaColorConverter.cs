using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.UI;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackToMetaColorConverter : MvxValueConverter<CellWrapperViewModel<Document>, UIColor>
    {
        protected override UIColor Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ViewModel is IDarkStyleOnIosViewModel)
                return UIColor.White.ColorWithAlpha(0.5f);

            return AppColors.LabelTertiaryColor;
        }
    }
}