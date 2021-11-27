using BMM.Core.Helpers;
using BMM.Core.Implementations.Player.Interfaces;

namespace BMM.Core.Implementations.Player
{
    public class RememberedQueueInfoService : IRememberedQueueInfoService
    {
        private readonly IDeepLinkHandler _deepLinkHandler;

        public RememberedQueueInfoService(IDeepLinkHandler deepLinkHandler)
        {
            _deepLinkHandler = deepLinkHandler;
        }

        public bool PreventRecoveringQueue { get; private set; }

        public void SetPendingDeepLink(string unhandledDeepLink)
        {
            PreventRecoveringQueue = _deepLinkHandler.DeepLinkStartsPlaying(unhandledDeepLink);
        }
        
        public void NotifyAfterRecoveringQueue()
        {
            PreventRecoveringQueue = false;
        }
    }
}