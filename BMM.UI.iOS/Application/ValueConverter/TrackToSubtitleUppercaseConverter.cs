using System;
using System.Globalization;
using BMM.Api.Abstraction;
using BMM.Core.ValueConverters;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class TrackToSubtitleUppercaseConverter: MvxValueConverter<ITrackModel, string>
    {
        private readonly TrackToSubtitleValueConverter _trackToSubtitleValueConverter;
        private readonly ToUppercaseConverter _toUppercaseConverter;

        public TrackToSubtitleUppercaseConverter()
        {
            _trackToSubtitleValueConverter = new TrackToSubtitleValueConverter();
            _toUppercaseConverter = new ToUppercaseConverter();
        }

        protected override string Convert(ITrackModel value, Type targetType, object parameter, CultureInfo culture)
        {
            var subtitle = _trackToSubtitleValueConverter.Convert(value, targetType, parameter, culture) as string;
            return _toUppercaseConverter.Convert(subtitle, targetType, parameter, culture) as string;
        }
    }
}