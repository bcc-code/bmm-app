using System;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.UI;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class TrackToStatusImageConverter : MvxValueConverter<Track, UIImage>
    {
        protected override UIImage Convert(Track currentTrack, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var wrapper = ((Func<CellWrapperViewModel<Document>>)parameter).Invoke();
            var document = wrapper.Item as Track;

            if (document == null)
                return null;

            if (TrackIsCurrentlySelected(document, currentTrack))
                return new UIImage("icon_now_playing");

            if (TrackIsNotListenedAndIsTeaserPodcast(document))
            {
                // returns the white version of the notification icon
                if (wrapper.ViewModel is IDarkStyleOnIosViewModel)
                    return new UIImage("notification_solid");

                return new UIImage("notification");
            }

            return null;
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

        private bool TrackIsCurrentlySelected(Document track, Document currentTrack)
        {
            return currentTrack != null && track.Id.Equals(currentTrack.Id);
        }
    }
}