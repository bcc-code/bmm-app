using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class TrackCollectionToSharingStateConverter : MvxValueConverter<TrackCollection>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        protected override object Convert(TrackCollection trackCollection, Type targetType, object parameter, CultureInfo culture)
        {
            if (!trackCollection.CanEdit)
                return TextSource.ConvertPlaylistAuthorToLabel(trackCollection.AuthorName);

            return trackCollection.FollowerCount == 0
                ? TextSource[Translations.TrackCollectionViewModel_Private]
                : string.Format(TextSource.GetText(Translations.ShareTrackCollectionViewModel_SharedWithFormat), trackCollection.FollowerCount);
        }
    }
}