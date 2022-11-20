using System.Collections.Generic;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Tracks.Interfaces
{
    public interface IPrepareTopSongsViewModelAction
        : IGuardedActionWithResult<IEnumerable<ITrackPO>>,
          IDataContextGuardedAction<ITopSongsCollectionViewModel>
    {
    }
}