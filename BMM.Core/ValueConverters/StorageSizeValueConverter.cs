using System;
using System.Globalization;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using MvvmCross.Localization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StorageSizeValueConverter : MvxValueConverter<IFileStorage, string>
    {
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(IFileStorage value, Type targetType, object parameter, CultureInfo culture)
        {
            var byteString = ByteToStringHelper.BytesToMegaBytes(value.UsableSpace);
            return BMMLanguageBinder.GetText(Translations.Global_BytesMB, byteString);
        }
    }
}