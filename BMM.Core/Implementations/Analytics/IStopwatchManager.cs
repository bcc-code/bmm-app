using System.Diagnostics;

namespace BMM.Core.Implementations.Analytics
{
    public interface IStopwatchManager
    {
        Stopwatch GetStopwatch(StopwatchType stopwatchType);
        Stopwatch StartAndGetStopwatch(StopwatchType stopwatchType);
    }
}