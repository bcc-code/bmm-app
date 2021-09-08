using System;
using System.Globalization;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class PlaylistAuthorToLabelConverter : MvxValueConverter<string>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        protected override object Convert(string authorName, Type targetType, object parameter, CultureInfo culture)
        {
            return TextSource.ConvertPlaylistAuthorToLabel(authorName);
        }
    }
}