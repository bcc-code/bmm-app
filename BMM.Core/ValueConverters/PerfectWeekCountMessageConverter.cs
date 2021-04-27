using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters
{
    public class PerfectWeekCountValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        private static IMvxLanguageBinder TextSource => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Streak");

        protected override string Convert(ListeningStreak streak, Type targetType, object parameter, CultureInfo culture)
        {
            if (streak.NumberOfPerfectWeeks <= 0)
                return TextSource.GetText("PerfectWeekCountNone");

            return TextSource.GetText(streak.NumberOfPerfectWeeks > 1 ? "PerfectWeekCountPlural" : "PerfectWeekCountSingular", streak.NumberOfPerfectWeeks);
        }
    }
}