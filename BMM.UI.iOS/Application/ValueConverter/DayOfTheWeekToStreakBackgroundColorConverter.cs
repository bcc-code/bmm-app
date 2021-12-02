using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.UI.iOS.Constants;
using MvvmCross.Converters;
using UIKit;

namespace BMM.UI.iOS
{
    public class DayOfTheWeekToStreakBackgroundColorConverter : MvxValueConverter<ListeningStreak, UIColor>
    {
        protected override UIColor Convert(ListeningStreak streak, Type targetType, object parameter, CultureInfo culture)
        {
            if (streak.DayOfTheWeek.ToString() == (string)parameter && DateTime.UtcNow < streak.EligibleUntil.ToUniversalTime())
                return AppColors.BackgroundSecondaryColor;

            return UIColor.Clear;
        }
    }
}