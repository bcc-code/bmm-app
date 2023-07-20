using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.ExploreNewest.Interfaces;
using BMM.Core.Models.POs.Base;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.ListeningStreaks
{
    public class ListeningStreakPO : DocumentPO
    {
        public ListeningStreakPO(
            IListeningStreakClickedAction listeningStreakClickedAction,
            ListeningStreak listeningStreak) : base(listeningStreak)
        {
            ListeningStreak = listeningStreak;
            ListeningStreakClickedCommand = new MvxAsyncCommand(async () =>
            {
                await listeningStreakClickedAction.ExecuteGuarded(ListeningStreak);
            });
        }
        
        public ListeningStreak ListeningStreak { get; }
        public IMvxAsyncCommand ListeningStreakClickedCommand { get; }
    }
}