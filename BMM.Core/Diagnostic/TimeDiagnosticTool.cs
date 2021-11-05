using System;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Core.Diagnostic.Interfaces;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Diagnostic
{
    public class TimeDiagnosticTool : ITimeDiagnosticTool
    {
        private readonly IAnalytics _analytics;

        public TimeDiagnosticTool(IAnalytics analytics)
        {
            _analytics = analytics;
        }

        public async Task LogIfConditionIsTrueAfterSpecifiedTime(
            Func<bool> condition,
            int timeInMillis,
            string errorTag)
        {
            await Task.Delay(timeInMillis);

            if (condition.Invoke())
                _analytics.LogEvent(errorTag);
        }
    }
}