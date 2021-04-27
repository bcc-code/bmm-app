using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class TrackToPublishedDateValueConverter: MvxValueConverter<Track, string>
    {
        protected override string Convert (Track track, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return track.PublishedAt.ToNorwegianTime().ToString("D", culture.DateTimeFormat);
        }
    }
}

