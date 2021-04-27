using System;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters
{
    public class TimeSpanToCountdownValueConverter: MvxValueConverter<TimeSpan?, string>
    {
        readonly MvxLanguageBinder _languageBinder = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(ExploreNewestViewModel));

        protected override string Convert (TimeSpan? timeLeft, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (timeLeft == null)
                return "";

            if (timeLeft.Value.TotalHours > 1)
            {
                return _languageBinder.GetText("CountdownHours", timeLeft.Value.Hours);
            }

            return $"{timeLeft.Value.Minutes}:{timeLeft.Value.Seconds.ToString().PadLeft(2, '0')}";
        }
    }
}