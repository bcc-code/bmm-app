using System.Collections.Generic;
using System.Diagnostics;

namespace BMM.Core.Implementations.Analytics
{
    public class StopwatchManager : IStopwatchManager
    {
        private readonly IDictionary<StopwatchType, Stopwatch> _stopwatches = new Dictionary<StopwatchType, Stopwatch>();

        public Stopwatch GetStopwatch(StopwatchType stopwatchType)
        {
            Stopwatch stopwatch;

            if (_stopwatches.ContainsKey(stopwatchType))
            {
                _stopwatches.TryGetValue(stopwatchType, out stopwatch);
            }
            else
            {
                stopwatch = new Stopwatch();
                _stopwatches.Add(stopwatchType, stopwatch);
            }

            return stopwatch;
        }

        public Stopwatch StartAndGetStopwatch(StopwatchType stopwatchType)
        {
            var stopwatch = GetStopwatch(stopwatchType);

            stopwatch.Reset();
            stopwatch.Start();

            return stopwatch;
        }
    }
}