using System;
using System.Timers;

namespace BMM.Core.Utils
{
    public class ThrottlingDispatcher : IDisposable
    {
        private const int IntervalsCountToDispose = 10;
        private int _currentEmptyIntervalsCount;
        private readonly int _throttlingInterval;
        private Timer _timer;
        private Action _action;

        public ThrottlingDispatcher(int throttlingInterval)
        {
            _throttlingInterval = throttlingInterval;
        }

        public void Run(Action action)
        {
            _currentEmptyIntervalsCount = 0;
            _action = action;

            if (_timer != null)
                return;
            
            _timer = new Timer(_throttlingInterval);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_action != null)
            {
                _action?.Invoke();
                _action = null;
                return;
            }

            _currentEmptyIntervalsCount++;
                
            if (_currentEmptyIntervalsCount == IntervalsCountToDispose)
                Dispose();
        }

        public void Dispose()
        {
            _action = null;
            
            if (_timer == null)
                return;

            _timer.Stop();
            _timer.Elapsed -= TimerOnElapsed;
            _timer.Dispose();
            _timer = null;
        }
    }
}