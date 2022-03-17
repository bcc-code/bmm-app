using System;
using System.Globalization;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class IsTrackPlayingToVisibleConverter : MvxValueConverter<Track, bool>
    {
        protected override bool Convert(Track track, Type targetType, object currentTrack, CultureInfo culture)
        {
            return TrackIsCurrentlySelected(track, currentTrack);
        }
        
        private bool TrackIsCurrentlySelected(Track track, object currentTrack) 
            => currentTrack is ITrackModel trackModel && track.Id.Equals(trackModel.Id);
    }
}