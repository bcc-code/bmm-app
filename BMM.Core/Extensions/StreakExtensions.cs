﻿using System;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Extensions
{
    public static class StreakExtensions
    {
        public static bool IsTodayAlreadyListened(this ListeningStreak streak)
        {
            switch (streak.DayOfTheWeek)
            {
                case DayOfWeek.Monday:
                    return streak.Monday == true;
                case DayOfWeek.Tuesday:
                    return streak.Tuesday == true;
                case DayOfWeek.Wednesday:
                    return streak.Wednesday == true;
                case DayOfWeek.Thursday:
                    return streak.Thursday == true;
                case DayOfWeek.Friday:
                    return streak.Friday == true;
                default:
                    return false;
            }
        }

        public static void MarkTodayAsListened(this ListeningStreak streak)
        {
            streak.LastChanged = DateTime.UtcNow;
            streak.DaysInARow++;
            switch (streak.DayOfTheWeek)
            {
                case DayOfWeek.Monday:
                    streak.Monday = true;
                    break;
                case DayOfWeek.Tuesday:
                    streak.Tuesday = true;
                    break;
                case DayOfWeek.Wednesday:
                    streak.Wednesday = true;
                    break;
                case DayOfWeek.Thursday:
                    streak.Thursday = true;
                    break;
                case DayOfWeek.Friday:
                    streak.Friday = true;
                    streak.IsPerfectWeek = streak.Monday == true && streak.Tuesday == true && streak.Wednesday == true && streak.Thursday == true;
                    if (streak.IsPerfectWeek)
                        streak.NumberOfPerfectWeeks++;
                    break;
                default:
                    break;
            }
        }
        
        public static bool IsEligible(this ListeningStreak streak)
        {
            return DateTime.UtcNow < streak.EligibleUntil.ToUniversalTime();
        }
    }
}
