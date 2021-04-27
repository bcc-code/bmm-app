using System;
using System.Globalization;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using MvvmCross.Localization;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StorageSizeValueConverter : MvxValueConverter<IFileStorage, string>
    {
        readonly MvxLanguageBinder _languageBinder = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Global");

        protected override string Convert(IFileStorage value, Type targetType, object parameter, CultureInfo culture)
        {
            var byteString = ByteToStringHelper.BytesToMegaBytes(value.UsableSpace);

            return _languageBinder.GetText("BytesMB", byteString);
        }
    }
}
