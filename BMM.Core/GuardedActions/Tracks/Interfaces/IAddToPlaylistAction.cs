using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.GuardedActions.Tracks.Parameters;

namespace BMM.Core.GuardedActions.Tracks.Interfaces;

public interface IAddToPlaylistAction : IGuardedActionWithParameter<TrackActionsParameter>
{
}