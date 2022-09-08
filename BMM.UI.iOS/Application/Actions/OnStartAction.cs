using System.Threading.Tasks;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;

namespace BMM.UI.iOS.Actions
{
    /// <summary>
    ///     Action that runs directly after app opening.
    /// </summary>
    public class OnStartAction : GuardedAction, IOnStartAction
    {
        private readonly IAVPlayerItemRepository _avPlayerItemRepository;

        public OnStartAction(IAVPlayerItemRepository avPlayerItemRepository)
        {
            _avPlayerItemRepository = avPlayerItemRepository;
        }
        
        protected override Task Execute()
        {
            _avPlayerItemRepository.SynchronizeCacheFiles();
            return Task.CompletedTask;
        }
    }
}