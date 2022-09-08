using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.DebugInfo.Interfaces;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using ByteSizeLib;

namespace BMM.UI.iOS.Actions.DebugInfo
{
    public class ShowTracksCacheInfoAction : GuardedAction, IShowTracksCacheInfoAction
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IAVPlayerItemRepository _avPlayerItemRepository;

        public ShowTracksCacheInfoAction(
            IUserDialogs userDialogs,
            IAVPlayerItemRepository avPlayerItemRepository)
        {
            _userDialogs = userDialogs;
            _avPlayerItemRepository = avPlayerItemRepository;
        }
        
        protected override async Task Execute()
        {
            var items = _avPlayerItemRepository.GetAllCachedFiles(false);

            var infoMessage = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                string file = items[i];
                infoMessage.AppendLine($"{i + 1}. {file} {ByteSize.FromBytes(file.GetCachePlayerItemExpectedSize()).MegaBytes:0.##} MB");
                infoMessage.AppendLine();
            }

            string messageToReturn = infoMessage.ToString();
            
            if (string.IsNullOrEmpty(messageToReturn))
                messageToReturn = "No cached tracks found.";
            
            await _userDialogs.AlertAsync(messageToReturn);
        }
    }
}