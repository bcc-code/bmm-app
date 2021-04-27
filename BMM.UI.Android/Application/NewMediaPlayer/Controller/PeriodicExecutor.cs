using System;
using Java.Lang;
using Java.Util.Concurrent;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Controller
{
    public class PeriodicExecutor : IDisposable
    {
        private const long ProgressUpdateInitialInterval = 100;
        private const long ProgressUpdateInternal = 1000;

        private readonly IScheduledExecutorService _scheduledExecutor;
        private IScheduledFuture _scheduledFuture;

        public PeriodicExecutor()
        {
            _scheduledExecutor = Executors.NewSingleThreadScheduledExecutor();
        }

        public void SchedulePeriodicExecution(Action action)
        {
            if (!_scheduledExecutor.IsShutdown)
            {
                _scheduledFuture = _scheduledExecutor.ScheduleAtFixedRate(new Runnable(action), ProgressUpdateInitialInterval, ProgressUpdateInternal, TimeUnit.Milliseconds);
            }
        }

        public void StopStateUpdater()
        {
            _scheduledFuture?.Cancel(true);
        }

        public void Dispose()
        {
            _scheduledExecutor.Shutdown();
        }
    }
}