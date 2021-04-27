using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class CurrentDayColorValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        protected override string Convert(ListeningStreak streak, Type targetType, object parameter, CultureInfo culture)
        {
            int dayAsInt = (int)(long)parameter;
            var dayToCheck = (DayOfWeek)dayAsInt;
            if (streak.DayOfTheWeek == dayToCheck && DateTime.UtcNow < streak.EligibleUntil.ToUniversalTime())
                return "#ECF0F3";

            return null;
        }
    }
}