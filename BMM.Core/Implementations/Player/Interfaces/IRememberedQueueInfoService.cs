namespace BMM.Core.Implementations.Player.Interfaces
{
    public interface IRememberedQueueInfoService
    {
        bool PreventRecoveringQueue { get; }
        void SetPlayerHasPendingOperation();
        void NotifyAfterRecoveringQueue();
    }
}