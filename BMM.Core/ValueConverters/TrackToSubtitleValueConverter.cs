using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class TrackToSubtitleValueConverter : MvxValueConverter<Track, string>
    {
        protected override string Convert(Track track, Type targetType, object parameter, CultureInfo culture)
        {
            var viewmodel = (PlayerBaseViewModel)parameter;
            return viewmodel.TrackInfoProvider.GetTrackInformation(track, culture)?.Subtitle;
        }
    }
}
