using System;
using System.Globalization;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters.TrackCollections
{
    public class PlaylistAuthorToLabelConverter : MvxValueConverter<string>
    {
        private readonly IMvxLanguageBinder _languageBinder =
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(MyContentViewModel));

        protected override object Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return string.Format(_languageBinder.GetText("ByFormat"), value);
        }
    }
}