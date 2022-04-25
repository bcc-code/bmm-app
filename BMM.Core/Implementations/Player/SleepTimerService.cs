using System;
using System.Timers;
using BMM.Core.GuardedActions.TrackOptions.Parameters;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.Implementations.Player
{
    public class SleepTimerService : ISleepTimerService
    {
        private readonly IMediaPlayer _mediaPlayer;
        private Timer _timer;
        private SleepTimerOption _currentOption;

        public SleepTimerService(IMediaPlayer mediaPlayer)
        {
            _mediaPlayer = mediaPlayer;
        }
        
        public void Set(SleepTimerOption sleepTimerOption)
        {
            Dispose();
            _currentOption = sleepTimerOption;
            _timer = new Timer(TimeSpan.FromMinutes((int)sleepTimerOption).TotalMilliseconds);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        public void Disable()
        {
            Dispose();
            _currentOption = SleepTimerOption.NotSet;
        }
        
        public void Dispose()
        {
            if (_timer == null)
                return;

            _timer.Stop();
            _timer.Elapsed -= TimerOnElapsed;
            _timer.Dispose();
            _timer = null;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _mediaPlayer.Pause();
            Dispose();
        }

        public bool IsEnabled => _currentOption != SleepTimerOption.NotSet;
        public SleepTimerOption CurrentSleepTimerOption => _currentOption;
    }
}