using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.ExploreNewest.Interfaces;
using BMM.Core.Models.POs.ListeningStreaks;

namespace BMM.Core.Implementations.Factories.Streak
{
    public class ListeningStreakPOFactory : IListeningStreakPOFactory
    {
        private readonly IListeningStreakClickedAction _listeningStreakClickedAction;

        public ListeningStreakPOFactory(IListeningStreakClickedAction listeningStreakClickedAction)
        {
            _listeningStreakClickedAction = listeningStreakClickedAction;
        }
        
        public ListeningStreakPO Create(ListeningStreak listeningStreak)
        {
            return new ListeningStreakPO(_listeningStreakClickedAction, listeningStreak);
        }
    }
}