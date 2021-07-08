using System;
using System.Globalization;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using MvvmCross.Localization;
using MvvmCross.UI;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class FollowersCountToMakePrivateButtonIsVisibleConverter : MvxValueConverter<int>
    {
        private readonly IMvxLanguageBinder _languageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(ShareTrackCollectionViewModel));

        protected override object Convert(int followersCount, Type targetType, object parameter, CultureInfo culture)
        {
            return followersCount != 0;
        }
    }
}