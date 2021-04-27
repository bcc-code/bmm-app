using System;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.PlayObserver.Model;

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
            if (uniqueSecondsListened == trackLength)
                status = ListenedStatus.Complete;

            return new PlayMeasurements
            {
                UniqueSecondsListened = uniqueSecondsListened,
                Status = status,
                Percentage = uniqueSecondsListened * 100 / trackLength,
                TrackLength = trackLength,
                TimestampStart = portions.First().StartTime,
                TimestampEnd = portions.Last().EndTime,
                SpentTime = CalculateSpentTime(portions)
            };
        }

        public double CalculateSpentTime(IList<ListenedPortion> portions)
        {
            return TimeSpan.FromMilliseconds(portions.Sum(portion => portion.End - portion.Start)).TotalSeconds;
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
                        if (endedListening - startedListening >= MinPortionDurationInMs)
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
            if (endedListening - startedListening >= MinPortionDurationInMs)
            {
                msListened += endedListening - startedListening;
            }

            var uniqueSecondsListened = Math.Ceiling(TimeSpan.FromMilliseconds(msListened).TotalSeconds);
            return uniqueSecondsListened;
        }
    }
}