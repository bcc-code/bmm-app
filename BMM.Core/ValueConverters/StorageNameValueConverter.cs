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
    public class StorageNameValueConverter : MvxValueConverter<IFileStorage, string>
    {
        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        protected override string Convert(IFileStorage value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.StorageKind == StorageKind.External
                ? TextSource.GetText(Translations.Global_External)
                : TextSource.GetText(Translations.Global_Internal);
        }
    }
}