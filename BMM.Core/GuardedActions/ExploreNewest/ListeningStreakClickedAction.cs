using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.ExploreNewest.Interfaces;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.ExploreNewest
{
    public class ListeningStreakClickedAction
        : GuardedActionWithParameter<ListeningStreak>,
          IListeningStreakClickedAction
    {
        private readonly IMvxNavigationService _mvxNavigationService;

        public ListeningStreakClickedAction(IMvxNavigationService mvxNavigationService)
        {
            _mvxNavigationService = mvxNavigationService;
        }

        protected override async Task Execute(ListeningStreak parameter)
        {
            await _mvxNavigationService.Navigate<ListeningStreakDetailsViewModel, IListeningStreakDetailsParameter>(
                new ListeningStreakDetailsParameter(parameter));
        }
    }
}