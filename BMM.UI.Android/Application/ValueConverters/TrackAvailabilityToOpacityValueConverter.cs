using System;
using System.Globalization;
using BMM.Core.Models.POs.Tracks;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class TrackAvailabilityToOpacityValueConverter : MvxValueConverter<TrackState, float>
    {
        private const float FullOpacity = 1f;
        private const float HalfOpacity = .5f;

        protected override float Convert(TrackState trackState, Type targetType, object parameter, CultureInfo culture)
        {
            if (trackState.IsAvailable)
                return FullOpacity;

            return HalfOpacity;
        }
    }
}