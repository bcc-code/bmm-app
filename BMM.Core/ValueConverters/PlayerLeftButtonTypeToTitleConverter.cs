using System;
using System.Globalization;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.Enums;
using BMM.Core.Translation;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class PlayerLeftButtonTypeToTitleConverter : MvxValueConverter<PlayerLeftButtonType, string>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(PlayerLeftButtonType value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == PlayerLeftButtonType.BCCMedia)
                return TextSource[Translations.PlayerViewModel_WatchOnBCCMedia];

            return TextSource[Translations.PlayerViewModel_ViewLyrics];
        }
    }
}