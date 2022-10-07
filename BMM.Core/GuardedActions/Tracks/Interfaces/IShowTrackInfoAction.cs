using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.Tracks.Interfaces
{
    public interface IShowTrackInfoAction : IGuardedActionWithParameter<Track>
    {
    }
}