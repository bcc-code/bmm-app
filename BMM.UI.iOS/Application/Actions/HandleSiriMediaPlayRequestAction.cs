using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.UI.iOS.Actions.Interfaces;
using Intents;

namespace BMM.UI.iOS.Actions
{
    public class HandleSiriMediaPlayRequestAction 
        : GuardedActionWithParameterAndResult<INPlayMediaIntent, INPlayMediaIntentResponseCode>,
          IHandleSiriMediaPlayRequestAction
    {
        private readonly IPlayMusicFromSiriAction _playMusicFromSiriAction;
        private readonly ISearchForMusicFromSiriAction _searchForMusicFromSiriAction;

        public HandleSiriMediaPlayRequestAction(
            IPlayMusicFromSiriAction playMusicFromSiriAction,
            ISearchForMusicFromSiriAction searchForMusicFromSiriAction)
        {
            _playMusicFromSiriAction = playMusicFromSiriAction;
            _searchForMusicFromSiriAction = searchForMusicFromSiriAction;
        }
        
        protected override async Task<INPlayMediaIntentResponseCode> Execute(INPlayMediaIntent playMediaIntent)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(playMediaIntent.MediaSearch?.MediaName))
                result = await _searchForMusicFromSiriAction.ExecuteGuarded(playMediaIntent.MediaSearch.MediaName);
            else if (playMediaIntent.MediaSearch!.MediaType == INMediaItemType.Music)
                result = await _playMusicFromSiriAction.ExecuteGuarded();

            return result
                ? INPlayMediaIntentResponseCode.Success
                : INPlayMediaIntentResponseCode.FailureUnknownMediaType;
        }
    }
}