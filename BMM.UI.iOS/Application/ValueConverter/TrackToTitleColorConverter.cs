using System;
using System.Globalization;
using BMM.Core.Models.POs.Tracks;
using BMM.UI.iOS.Constants;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackToTitleColorConverter: MvxValueConverter<TrackState, UIColor>
    {
        protected override UIColor Convert(TrackState trackState, Type targetType, object parameter, CultureInfo culture)
        {
            if (trackState.IsCurrentlySelected)
                return AppColors.TintColor;

            if (!trackState.IsAvailable)
                return UIColor.Gray;
            
            return AppColors.LabelOneColor;
        }
    }
}