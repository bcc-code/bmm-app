using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.ExploreNewest.Interfaces
{
    public interface IListeningStreakClickedAction : IGuardedActionWithParameter<ListeningStreak>
    {
    }
}