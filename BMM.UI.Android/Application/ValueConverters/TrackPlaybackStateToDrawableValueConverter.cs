using System;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.UI;
using BMM.Core.ViewModels;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class TrackPlaybackStateToDrawableValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, int>
    {
        protected override int Convert(CellWrapperViewModel<Document> document, Type targetType, object currentTrack, System.Globalization.CultureInfo culture)
        {
            var item = document.Item;
            var track = (Track)item;

            if (TrackIsCurrentlySelected(track, currentTrack))
                return Resource.Drawable.icon_now_playing;

            if (TrackIsNotListenedAndIsTeaserPodcast(track))
                return document.ViewModel is IDarkStyleOnAndroidViewModel ? Resource.Drawable.notification_solid : Resource.Drawable.icon_blue_dot;

            return 0;
        }

        private bool TrackIsCurrentlySelected(Track track, object currentTrack)
        {
            return currentTrack is ITrackModel trackModel && track.Id.Equals(trackModel.Id);
        }

        private bool TrackIsNotListenedAndIsTeaserPodcast(Track track)
        {
            return !track.IsListened && TrackIsTeaserPodcast(track);
        }

        private bool TrackIsTeaserPodcast(Track track)
        {
            return track.Tags.Contains(FraKaareTeaserViewModel.FromKaareTagName) ||
                   track.Tags.Contains(AslaksenTeaserViewModel.AsklaksenTagName) ||
                   track.Tags.Contains(AslaksenTeaserViewModel.HebrewTagName);
        }
    }
}
