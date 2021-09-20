using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.PlaybackHistory.Interfaces
{
    public interface IPreparePlaybackHistoryAction
        : IGuardedActionWithResult<IEnumerable<Document>>
    {
    }
}