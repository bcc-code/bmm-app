using System;
using System.Globalization;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.Enums;
using BMM.Core.Translation;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class PlayerLeftButtonTypeToTitleConverter : MvxValueConverter<PlayerLeftButtonType?, string>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(PlayerLeftButtonType? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                PlayerLeftButtonType.Lyrics => TextSource[Translations.PlayerViewModel_ViewLyrics],
                PlayerLeftButtonType.Transcription => TextSource[Translations.PlayerViewModel_Read],
                null => string.Empty
            };
        }
    }
}