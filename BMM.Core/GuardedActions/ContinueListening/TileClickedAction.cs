using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.GuardedActions.ContinueListening
{
    public class TileClickedAction
        : GuardedActionWithParameter<ContinueListeningTile>,
          ITileClickedAction
    {
        private readonly IDeepLinkHandler _deepLinkHandler;

        public TileClickedAction(IDeepLinkHandler deepLinkHandler)
        {
            _deepLinkHandler = deepLinkHandler;
        }
        
        protected override Task Execute(ContinueListeningTile parameter)
        {
            _deepLinkHandler.OpenFromInsideOfApp(parameter.ShowAllLink, PlaybackOrigins.Tile);
            return Task.CompletedTask;
        }
    }
}