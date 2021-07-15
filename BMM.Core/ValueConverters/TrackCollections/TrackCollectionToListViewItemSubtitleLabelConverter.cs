using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToListViewItemSubtitleLabelConverter : MvxValueConverter<TrackCollection, string>
    {
        private readonly IMvxLanguageBinder _myContentViewModelLanguageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(MyContentViewModel));

        private readonly IMvxLanguageBinder _documentsViewModelLanguageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(DocumentsViewModel));

        protected override string Convert(TrackCollection trackCollection, Type targetType, object parameter, CultureInfo culture)
        {
            if (trackCollection.CanEdit)
                return string.Format(_documentsViewModelLanguageBinder.GetText("PluralTracks"), trackCollection.TrackCount);

            if (string.IsNullOrEmpty(trackCollection.AuthorName))
                return string.Empty;

            return string.Format(_myContentViewModelLanguageBinder.GetText("ByFormat"), trackCollection.AuthorName);
        }
    }
}