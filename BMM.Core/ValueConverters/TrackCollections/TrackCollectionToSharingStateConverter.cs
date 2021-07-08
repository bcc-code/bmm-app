using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToSharingStateConverter : MvxValueConverter<TrackCollection>
    {
        private readonly IMvxLanguageBinder _languageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(ShareTrackCollectionViewModel));

        protected override object Convert(TrackCollection trackCollection, Type targetType, object parameter, CultureInfo culture)
        {
            if (!trackCollection.CanEdit)
                return $"{_languageBinder.GetText("By")} {trackCollection.AuthorName}";

            return trackCollection.FollowerCount == 0
                ? _languageBinder.GetText("Private")
                : string.Format(_languageBinder.GetText("SharedWithFormat"), trackCollection.FollowerCount);
        }
    }
}