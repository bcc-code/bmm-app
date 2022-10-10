using System;
using System.Globalization;
using System.Linq;
using BMM.Api.Implementation.Models;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class TrackHasExternalRelationsValueConverter: MvxValueConverter<Track, bool>
    {
        protected override bool Convert(Track track, Type targetType, object parameter, CultureInfo culture)
        {
            return track.Relations?.Any(relation => relation.Type == TrackRelationType.External) ?? false;
        }
    }
}