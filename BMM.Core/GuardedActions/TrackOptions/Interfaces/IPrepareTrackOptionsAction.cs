using System.Collections.Generic;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.GuardedActions.TrackOptions.Parameters.Interfaces;
using BMM.Core.Models.POs;

namespace BMM.Core.GuardedActions.TrackOptions.Interfaces
{
    public interface IPrepareTrackOptionsAction : IGuardedActionWithParameterAndResult<IPrepareTrackOptionsParameters, IList<StandardIconOptionPO>>
    {
    }
}