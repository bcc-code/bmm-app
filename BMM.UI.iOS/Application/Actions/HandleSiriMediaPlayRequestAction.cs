using System.Linq;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Security;
using BMM.UI.iOS.Actions.Interfaces;
using BMM.UI.iOS.Constants;
using Intents;

namespace BMM.UI.iOS.Actions
{
    public class HandleSiriMediaPlayRequestAction 
        : GuardedActionWithParameterAndResult<INPlayMediaIntent, INPlayMediaIntentResponseCode>,
          IHandleSiriMediaPlayRequestAction
    {
        private readonly IPlayMusicFromSiriAction _playMusicFromSiriAction;
        private readonly ISearchForMusicFromSiriAction _searchForMusicFromSiriAction;
        private readonly IPlayFromKareAction _playFromKareAction;
        private readonly IUserStorage _userStorage;

        public HandleSiriMediaPlayRequestAction(
            IPlayMusicFromSiriAction playMusicFromSiriAction,
            ISearchForMusicFromSiriAction searchForMusicFromSiriAction,
            IPlayFromKareAction playFromKareAction,
            IUserStorage userStorage)
        {
            _playMusicFromSiriAction = playMusicFromSiriAction;
            _searchForMusicFromSiriAction = searchForMusicFromSiriAction;
            _playFromKareAction = playFromKareAction;
            _userStorage = userStorage;
        }
        
        protected override async Task<INPlayMediaIntentResponseCode> Execute(INPlayMediaIntent playMediaIntent)
        {
            bool result = false;

            if (!_userStorage.HasUser())
                return INPlayMediaIntentResponseCode.ContinueInApp;

            var mediaItem = playMediaIntent.MediaItems?.FirstOrDefault();
            
            if (mediaItem?.Identifier == SiriConstants.FromKareIdentifier)
                result = await _playFromKareAction.ExecuteGuarded();
            else if (mediaItem?.Identifier == SiriConstants.PlayMusicIdentifier)
                result = await _playMusicFromSiriAction.ExecuteGuarded();
            else if (!string.IsNullOrEmpty(playMediaIntent.MediaSearch?.MediaName))
                result = await _searchForMusicFromSiriAction.ExecuteGuarded(playMediaIntent.MediaSearch.MediaName);
            else if (playMediaIntent.MediaSearch?.MediaType == INMediaItemType.Music)
                result = await _playMusicFromSiriAction.ExecuteGuarded();

            return result
                ? INPlayMediaIntentResponseCode.Success
                : INPlayMediaIntentResponseCode.FailureUnknownMediaType;
        }
    }
}