using System;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.NewMediaPlayer.Constants;

namespace BMM.Core.Implementations.PlayObserver
{
    public class MeasurementCalculator : IMeasurementCalculator
    {
        // Normally, the track starts at second 0 and the user can seek to another position.
        // We don't want to log a portion which wasn't actually meant to be listened.
        // That's why we have set a minPortionDurationInMs which takes care of those intervals.
        private const long MinPortionDurationInMs = 100;

        public PlayMeasurements Calculate(long trackDuration, IList<ListenedPortion> portions)
        {
            if (!portions.Any())
            {
                return null;
            }

            var uniqueSecondsListened = CalculateUniqueSecondsListened(portions, out ListenedStatus status);
            if (uniqueSecondsListened <= 0)
                return null;

            var trackLength = TimeSpan.FromMilliseconds(trackDuration).TotalSeconds;
            if (Math.Abs(uniqueSecondsListened - trackLength) < PlayStatistics.TimeCompareToleranceInMillis)
                status = ListenedStatus.Complete;

            return new PlayMeasurements
            {
                UniqueSecondsListened = uniqueSecondsListened,
                Status = status,
                Percentage = uniqueSecondsListened * 100 / trackLength,
                TrackLength = trackLength,
                TimestampStart = portions.First().StartTime,
                TimestampEnd = portions.Last().EndTime,
                SpentTime = CalculateSpentTime(portions),
                LastPosition = GetLastPosition(portions),
                AdjustedPlaybackSpeed = GetAdjustedPlaybackSpeed(portions)
            };
        }

        private decimal GetAdjustedPlaybackSpeed(IList<ListenedPortion> portions)
        {
            var lastPortionWithUnusualPlaybackSpeed = portions
                .OrderByDescending(p => p.EndTime)
                .FirstOrDefault(p => p.PlaybackRate != PlayerConstants.NormalPlaybackSpeed);

            return lastPortionWithUnusualPlaybackSpeed?.PlaybackRate ?? PlayerConstants.NormalPlaybackSpeed;
        }

        private long GetLastPosition(IList<ListenedPortion> portions)
        {
            var newestPortion = portions
                .OrderByDescending(p => p.EndTime)
                .First();

            return (long)newestPortion.End;
        }

        public double CalculateSpentTime(IList<ListenedPortion> portions)
        {
            double sumOfSpentTime = portions.Sum(portion => (portion.End - portion.Start) / (double)portion.PlaybackRate);
            return TimeSpan.FromMilliseconds(sumOfSpentTime).TotalSeconds;
        }

        public double CalculateUniqueSecondsListened(IList<ListenedPortion> portions, out ListenedStatus status)
        {
            double msListened = 0;
            status = ListenedStatus.PartialFromBeginning;

            var orderedPortions = portions.OrderBy(portion => portion.Start).ThenBy(portion => portion.End).ToList();

            double startedListening = orderedPortions.First().Start;
            double endedListening = orderedPortions.First().End;

            foreach (var portion in orderedPortions)
            {
                if (!portion.Start.Equals(startedListening))
                {
                    if (portion.Start > endedListening)
                    {
                        if (ShouldIncludePortion(endedListening, startedListening))
                        {
                            status = ListenedStatus.Jumped;
                            msListened += endedListening - startedListening;
                        }
                        else
                        {
                            if (startedListening == 0)
                                status = ListenedStatus.SkippedBeginning;
                        }

                        startedListening = portion.Start;
                    }
                }

                if (portion.End > endedListening) endedListening = portion.End;
            }

            // Add the last listened portion
            if (ShouldIncludePortion(endedListening, startedListening))
                msListened += endedListening - startedListening;

            return Math.Ceiling(TimeSpan.FromMilliseconds(msListened).TotalSeconds);
        }

        private static bool ShouldIncludePortion(double endedListening, double startedListening)
        {
            return endedListening - startedListening >= MinPortionDurationInMs;
        }
    }
}