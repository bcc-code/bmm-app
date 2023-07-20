using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.ListeningStreaks;

namespace BMM.Core.Implementations.Factories.Streak
{
    public interface IListeningStreakPOFactory
    {
        ListeningStreakPO Create(ListeningStreak listeningStreak);
    }
}