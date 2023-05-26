using System;
using System.Globalization;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackToSubtitleColorConverter: MvxValueConverter<TrackState, UIColor>
    {
        protected override UIColor Convert(TrackState trackState, Type targetType, object parameter, CultureInfo culture)
        {
            if (!trackState.IsAvailable)
                return UIColor.Gray;

            return AppColors.LabelTwoColor;

        }
    }
}