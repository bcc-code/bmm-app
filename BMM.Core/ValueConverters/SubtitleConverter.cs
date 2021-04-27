using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackInformation.Strategies;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class SubtitleConverter: MvxValueConverter<Document, string>
    {
        private readonly ITrackInfoProvider _infoProvider = new DefaultTrackInfoProvider();

        protected override string Convert(Document value, Type targetType, object parameter, CultureInfo culture)
        {
            var info = _infoProvider.GetTrackInformation(value as Track, culture);
            return info.Subtitle;
        }
    }
}