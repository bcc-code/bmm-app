using System.Threading.Tasks;
using MvvmCross.ViewModels;
using MvvmCross.Navigation;

namespace BMM.Core
{
    public class AppStart : MvxAppStart
    {
        private readonly IAppNavigator _appNavigator;

        public AppStart(IMvxApplication application, IMvxNavigationService navigationService, IAppNavigator appNavigator) : base(application, navigationService)
        {
            _appNavigator = appNavigator;
        }

        /// <summary>
        /// </summary>
        /// <param name="hint">Hint contains information in case the app is started with extra parameters</param>
        protected override async Task NavigateToFirstViewModel(object hint = null)
        {
            // todo make this maybe async after PR is merged: https://github.com/MvvmCross/MvvmCross/pull/3222
            _appNavigator.NavigateAtAppStart();
        }
    }
}