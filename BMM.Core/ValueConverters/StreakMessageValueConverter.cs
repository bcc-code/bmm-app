using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using MvvmCross.Converters;
using MvvmCross.Localization;

namespace BMM.Core.ValueConverters
{
    public class StreakMessageValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        private static IMvxLanguageBinder GlobalTextSource => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Streak");

        protected override string Convert(ListeningStreak week, Type targetType, object parameter, CultureInfo culture)
        {
            if (week.IsPerfectWeek)
                return GlobalTextSource.GetText("MessagePerfect");

            return GlobalTextSource.GetText("Message");
        }
    }
}