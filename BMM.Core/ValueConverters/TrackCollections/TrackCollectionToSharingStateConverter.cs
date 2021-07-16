using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToSharingStateConverter : MvxValueConverter<TrackCollection>
    {
        private readonly IMvxLanguageBinder _shareTrackCollectionLanguageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(ShareTrackCollectionViewModel));

        private readonly IMvxLanguageBinder _trackCollectionLanguageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(TrackCollectionViewModel));

        private readonly IMvxLanguageBinder _myContentLanguageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(MyContentViewModel));

        protected override object Convert(TrackCollection trackCollection, Type targetType, object parameter, CultureInfo culture)
        {
            if (!trackCollection.CanEdit)
                return string.Format(_myContentLanguageBinder.GetText("ByFormat"), trackCollection.AuthorName);

            return trackCollection.FollowerCount == 0
                ? _trackCollectionLanguageBinder.GetText("Private")
                : string.Format(_shareTrackCollectionLanguageBinder.GetText("SharedWithFormat"), trackCollection.FollowerCount);
        }
    }
}