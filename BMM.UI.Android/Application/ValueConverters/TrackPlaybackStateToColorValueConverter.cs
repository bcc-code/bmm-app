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
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Converters;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class TrackPlaybackStateToColorValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, Color>
    {
        private readonly IConnection _connection;
        private readonly IStorageManager _storageManager;

        public TrackPlaybackStateToColorValueConverter()
        {
            _connection = Mvx.IoCProvider.Resolve<IConnection>();
            _storageManager = Mvx.IoCProvider.Resolve<IStorageManager>();
        }

        protected override Color Convert(CellWrapperViewModel<Document> document, Type targetType, object currentTrack, CultureInfo culture)
        {
            var item = document.Item;
            var track = (Track)item;
            var context = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            var isDark = document.ViewModel is IDarkStyleOnAndroidViewModel;

            if (TrackHasPlayerFocus(track, currentTrack))
                return new Color(ContextCompat.GetColor(context, Resource.Color.colorPrimary));

            if (TrackIsNotAvailableOffline(track))
                return new Color(ContextCompat.GetColor(context, isDark ? Resource.Color.med_white : Resource.Color.med_black));

            return new Color(ContextCompat.GetColor(context, isDark ? Resource.Color.white : Resource.Color.black));
        }

        private bool TrackIsNotAvailableOffline(Track track)
        {
            var userIsOffline = _connection.GetStatus() == ConnectionStatus.Offline;
            var trackIsNotDownloaded = !_storageManager.SelectedStorage.IsDownloaded(track);
            return userIsOffline && trackIsNotDownloaded;
        }

        private bool TrackHasPlayerFocus(Track track, object currentTrack)
        {
            return currentTrack is ITrackModel trackModel && track.Id.Equals(trackModel.Id);
        }
    }
}