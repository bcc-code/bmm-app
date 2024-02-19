using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.TrackCollections.Interfaces;

namespace BMM.Core.GuardedActions.Tracklist.Interfaces;

public interface ILikeUnlikeTrackAction : IGuardedActionWithParameterAndResult<ILikeOrUnlikeTrackActionParameter, bool>
{
}