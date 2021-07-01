using System;
using System.Globalization;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class FollowersCountToTrackCollectionStateConverter : MvxValueConverter<int>
    {
        private readonly IMvxLanguageBinder _languageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(ShareTrackCollectionViewModel));

        protected override object Convert(int followersCount, Type targetType, object parameter, CultureInfo culture)
        {
            return followersCount == 0
                ? _languageBinder.GetText("Private")
                : string.Format(_languageBinder.GetText("SharedWithFormat"), followersCount);
        }
    }
}