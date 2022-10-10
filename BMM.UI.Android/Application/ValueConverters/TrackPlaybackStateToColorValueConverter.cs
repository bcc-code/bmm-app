using System;
using System.Diagnostics;
using System.Globalization;
using Android.Graphics;
using AndroidX.Core.Content;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Converters;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class TrackPlaybackStateToColorValueConverter : MvxValueConverter<TrackState, Color>
    {
        private readonly IConnection _connection;
        private readonly IStorageManager _storageManager;

        public TrackPlaybackStateToColorValueConverter()
        {
            _connection = Mvx.IoCProvider.Resolve<IConnection>();
            _storageManager = Mvx.IoCProvider.Resolve<IStorageManager>();
        }

        protected override Color Convert(TrackState trackState, Type targetType, object currentTrack, CultureInfo culture)
        {
            var context = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;

            if (trackState.IsCurrentlySelected)
                return new Color(ContextCompat.GetColor(context, Resource.Color.tint_color));

            if (!trackState.IsAvailable)
                return new Color(ContextCompat.GetColor(context,  Resource.Color.med_black));

            return new Color(ContextCompat.GetColor(context,Resource.Color.label_primary_color));
        }
    }
}