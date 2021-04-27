using System;
using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Implementations.LiveRadio
{
    public class LiveTime : ILiveTime
    {
        private readonly IAnalytics _analytics;
        private TimeSpan _difference;

        private const int MaximumSecondsOffAllowed = 10;

        public LiveTime(IAnalytics analytics)
        {
            _analytics = analytics;
        }

        /// <summary>
        /// Time on the server.
        /// If the local time is off by ten seconds it uses the time send in <see cref="LiveInfo"/>.
        /// </summary>
        public DateTime TimeOnServer => Math.Abs(_difference.TotalSeconds) > MaximumSecondsOffAllowed ? DateTime.Now - _difference : DateTime.Now;

        public void SetLiveInfo(LiveInfo liveInfo)
        {
            var difference = liveInfo.LocalTime.Subtract(liveInfo.ServerTime);

            if (liveInfo.Track != null &&
                Math.Abs(difference.TotalSeconds) > MaximumSecondsOffAllowed)
            {
                _analytics.LogEvent($"Device time is off with more than {MaximumSecondsOffAllowed} seconds",
                    new Dictionary<string, object> {{"offsetInSeconds", difference.TotalSeconds}});
                _difference = difference;
            }
            else
            {
                _difference = new TimeSpan();
            }
        }
    }
}