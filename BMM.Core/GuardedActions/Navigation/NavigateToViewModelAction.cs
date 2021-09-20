using System;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Navigation.Interfaces;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.Navigation
{
    public class NavigateToViewModelAction
        : GuardedActionWithParameter<Type>,
          INavigateToViewModelAction
    {
        private readonly IMvxNavigationService _mvxNavigationService;

        public NavigateToViewModelAction(IMvxNavigationService mvxNavigationService)
        {
            _mvxNavigationService = mvxNavigationService;
        }

        protected override Task Execute(Type vmType)
        {
            return _mvxNavigationService.Navigate(vmType);
        }
    }
}