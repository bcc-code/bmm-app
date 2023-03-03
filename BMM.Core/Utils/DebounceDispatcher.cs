using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BMM.Core.Utils
{
    public class DebounceDispatcher : IDisposable
    {
        private readonly int _delayDuration;
        private Timer _timer;
        private Action _action;

        public DebounceDispatcher(int delayDuration)
        {
            _delayDuration = delayDuration;
        }

        public void Run(Action action)
        {
            _action = action;

            if (_timer != null)
            {
                _timer.Stop();
            }
            else
            {
                _timer = new Timer(_delayDuration);
                _timer.Elapsed += TimerOnElapsed;
            }

            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            _action?.Invoke();
        }

        public void Dispose()
        {
            if (_timer == null)
                return;

            _timer.Elapsed -= TimerOnElapsed;
            _timer.Dispose();
            _timer = null;
        }
    }
}