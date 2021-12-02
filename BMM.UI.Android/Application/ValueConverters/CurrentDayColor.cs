using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross;
using MvvmCross.Converters;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class CurrentDayColorValueConverter : MvxValueConverter<ListeningStreak, string>
    {
        protected override string Convert(ListeningStreak streak, Type targetType, object parameter, CultureInfo culture)
        {
            var mvxAndroidCurrentTopActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>();
            int dayAsInt = (int)(long)parameter;
            var dayToCheck = (DayOfWeek)dayAsInt;
            
            if (streak.DayOfTheWeek == dayToCheck && DateTime.UtcNow < streak.EligibleUntil.ToUniversalTime())
                return mvxAndroidCurrentTopActivity.Activity.Resources!.GetString(Resource.Color.background_secondary_color);

            return null;
        }
    }
}