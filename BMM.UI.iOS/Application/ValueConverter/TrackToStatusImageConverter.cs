using System;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackToStatusImageConverter : MvxValueConverter<TrackState, UIImage>
    {
        protected override UIImage Convert(TrackState trackState, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (trackState.IsCurrentlySelected)
                return new UIImage("icon_now_playing");

            if (TrackIsNotListenedAndIsTeaserPodcast(trackState))
            {
                return new UIImage("notification");
            }

            return null;
        }

        private bool TrackIsNotListenedAndIsTeaserPodcast(TrackState trackState)
        {
            return !trackState.TrackIsListened && trackState.IsTeaserPodcast;
        }
    }
}