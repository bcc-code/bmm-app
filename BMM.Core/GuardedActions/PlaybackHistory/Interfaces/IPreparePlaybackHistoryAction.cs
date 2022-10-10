using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.PlaybackHistory.Interfaces
{
    public interface IPreparePlaybackHistoryAction
        : IGuardedActionWithResult<IEnumerable<IDocumentPO>>,
          IDataContextGuardedAction<IPlaybackHistoryViewModel>
    {
    }
}