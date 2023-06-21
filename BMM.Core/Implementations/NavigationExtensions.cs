using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Helpers.PresentationHints;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace BMM.Core.Implementations
{
    public static class NavigationExtensions
    {
        public static IMvxCommand NavigateCommand<TViewModel>(this IMvxNavigationService navigationService) where TViewModel : IMvxViewModel
        {
            return new ExceptionHandlingCommand(async () => await navigationService.Navigate<TViewModel>());
        }
        
        public static IMvxCommand NavigateCommand<TViewModel, TParameter>(this IMvxNavigationService navigationService, TParameter parameter) where TViewModel : IMvxViewModel<TParameter>
        {
            return new ExceptionHandlingCommand(async () => await navigationService.Navigate<TViewModel, TParameter>(parameter));
        }

        /// <summary>
        /// Navigate to ViewModel and set it as the new navigation root. This is supposed to be used in the menu.
        /// </summary>
        public static async Task NavigateToNewRoot<TViewModel>(this IMvxNavigationService navigationService) where TViewModel : IMvxViewModel
        {
            await navigationService.Navigate<TViewModel>();
            await navigationService.ChangePresentation(new NavigationRootChangedHint(typeof(TViewModel)));
        }
    }
}
