using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class ListeningStreakDetailsParameter : IListeningStreakDetailsParameter
    {
        public ListeningStreakDetailsParameter(ListeningStreak listeningStreak)
        {
            ListeningStreak = listeningStreak;
        }
        
        public ListeningStreak ListeningStreak { get; }
    }
}