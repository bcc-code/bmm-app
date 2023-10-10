using BMM.Core.Models.POs.Tracks;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class TrackToStatusImageConverter : MvxValueConverter<TrackState, UIImage>
    {
        protected override UIImage Convert(TrackState trackState, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (trackState.IsCurrentlySelected)
                return new UIImage("icon_now_playing");

            if (trackState.ShowBlueDot)
                return new UIImage("notification");

            return null;
        }
    }
}