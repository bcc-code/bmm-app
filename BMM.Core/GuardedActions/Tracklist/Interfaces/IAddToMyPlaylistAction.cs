using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Tracklist.Interfaces
{
    public interface IAddToMyPlaylistAction
        : IGuardedAction,
          IDataContextGuardedAction<ISharedTrackCollectionViewModel>
    {
    }
}