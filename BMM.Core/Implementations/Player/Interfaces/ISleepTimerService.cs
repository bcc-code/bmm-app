using BMM.Core.GuardedActions.TrackOptions.Parameters;

namespace BMM.Core.Implementations.Player.Interfaces
{
    public interface ISleepTimerService
    {
        void Set(SleepTimerOption sleepTimerOption);
        void Disable();

        bool IsEnabled { get; }
        SleepTimerOption CurrentSleepTimerOption { get; }
    }
}