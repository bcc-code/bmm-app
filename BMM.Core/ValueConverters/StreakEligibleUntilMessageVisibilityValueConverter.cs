using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StreakEligibleUntilMessageVisibilityValueConverter : MvxValueConverter<ListeningStreak, bool>
    {
        protected override bool Convert(ListeningStreak listeningStreak, Type targetType, object parameter, CultureInfo culture)
        {
            if (listeningStreak.IsTodayAlreadyListened() || listeningStreak.EligibleUntil < DateTime.UtcNow)
                return false;

            return true;
        }
    }
}