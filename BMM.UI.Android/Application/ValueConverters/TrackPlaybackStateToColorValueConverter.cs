using System;
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
            var userIsOffline = Mvx.IoCProvider.Resolve<IConnection>().GetStatus() == ConnectionStatus.Offline;
            var trackIsNotDownloaded = !Mvx.IoCProvider.Resolve<IStorageManager>().SelectedStorage.IsDownloaded(track);

            return userIsOffline && trackIsNotDownloaded;
        }

        private bool TrackHasPlayerFocus(Track track, object currentTrack)
        {
            return currentTrack is ITrackModel trackModel && track.Id.Equals(trackModel.Id);
        }
    }
}