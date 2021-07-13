using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToListViewItemSubtitleLabelConverter : MvxValueConverter<TrackCollection, string>
    {
        private readonly IMvxLanguageBinder _languageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(MyContentViewModel));

        protected override string Convert(TrackCollection trackCollection, Type targetType, object parameter, CultureInfo culture)
        {
            if (trackCollection.CanEdit)
                return string.Format(_languageBinder.GetText("TrackCountFormat"), trackCollection.TrackCount);

            if (string.IsNullOrEmpty(trackCollection.AuthorName))
                return string.Empty;

            return string.Format(_languageBinder.GetText("ByFormat"), trackCollection.AuthorName);
        }
    }
}