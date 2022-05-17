using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.ContinueListening.Interfaces
{
    public interface IResumeOrShufflePlayAction 
        : IGuardedAction,
          IDataContextGuardedAction<IAlbumViewModel>
    {
    }
}