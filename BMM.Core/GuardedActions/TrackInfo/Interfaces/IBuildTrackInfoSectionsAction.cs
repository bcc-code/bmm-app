using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.GuardedActions.TrackInfo.Interfaces;

public interface IBuildTrackInfoSectionsAction : IGuardedActionWithParameterAndResult<Track, IEnumerable<IBasePO>>
{
}