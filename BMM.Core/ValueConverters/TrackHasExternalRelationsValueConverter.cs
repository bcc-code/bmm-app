using System;
using System.Globalization;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class TrackHasExternalRelationsValueConverter: MvxValueConverter<Track, bool>
    {
        protected override bool Convert(Track track, Type targetType, object parameter, CultureInfo culture) => track.HasExternalRelations();
    }
}