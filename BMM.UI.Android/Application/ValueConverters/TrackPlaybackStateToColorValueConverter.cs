using System.Globalization;
using Android.Graphics;
using AndroidX.Core.Content;
using BMM.Core.Models.POs.Tracks;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross;
using MvvmCross.Converters;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class TrackPlaybackStateToColorValueConverter : MvxValueConverter<TrackState, Color>
    {
        protected override Color Convert(TrackState trackState, Type targetType, object currentTrack, CultureInfo culture)
        {
            var context = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;

            if (trackState.IsCurrentlySelected)
                return context.GetColorFromTheme(Resource.Attribute.tint_color);

            if (!trackState.IsAvailable)
                return new Color(ContextCompat.GetColor(context,  Resource.Color.med_black));

            return new Color(ContextCompat.GetColor(context,Resource.Color.label_one_color));
        }
    }
}