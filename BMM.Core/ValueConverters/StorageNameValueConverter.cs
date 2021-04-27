using System;
using System.Globalization;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using MvvmCross.Localization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StorageNameValueConverter : MvxValueConverter<IFileStorage, string>
    {
        private readonly MvxLanguageBinder _languageBinder = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Global");

        protected override string Convert(IFileStorage value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.StorageKind == StorageKind.External ? _languageBinder.GetText("External") : _languageBinder.GetText("Internal");
        }
    }
}
