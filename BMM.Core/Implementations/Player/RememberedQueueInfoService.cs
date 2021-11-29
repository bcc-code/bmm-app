using BMM.Core.Implementations.Player.Interfaces;

namespace BMM.Core.Implementations.Player
{
    public class RememberedQueueInfoService : IRememberedQueueInfoService
    {
        public bool PreventRecoveringQueue { get; private set; }

        public void SetPlayerHasPendingOperation()
        {
            PreventRecoveringQueue = true;
        }

        public void NotifyAfterRecoveringQueue()
        {
            PreventRecoveringQueue = false;
        }
    }
}