using System;
using BMM.Core.Models.POs.Tracks;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class TrackPlaybackStateToDrawableValueConverter : MvxValueConverter<TrackState, int>
    {
        protected override int Convert(TrackState trackState, Type targetType, object currentTrack, System.Globalization.CultureInfo culture)
        {
            if (trackState.IsCurrentlySelected)
                return Resource.Drawable.icon_now_playing;

            if (TrackIsNotListenedAndIsTeaserPodcast(trackState))
                return  Resource.Drawable.icon_blue_dot;

            return 0;
        }

        private bool TrackIsNotListenedAndIsTeaserPodcast(TrackState trackState)
        {
            return !trackState.TrackIsListened && trackState.IsTeaserPodcast;
        }
    }
}
