using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToListViewItemSubtitleLabelConverter : MvxValueConverter<TrackCollection, string>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(TrackCollection trackCollection, Type targetType, object parameter, CultureInfo culture)
        {
            if (trackCollection.CanEdit)
                return string.Format(TextSource[Translations.DocumentsViewModel_PluralTracks], trackCollection.TrackCount);

            return TextSource.ConvertPlaylistAuthorToLabel(trackCollection.AuthorName);
        }
    }
}