using Acr.UserDialogs;
using BMM.Core.Implementations;
using BMM.Core.Implementations.UI;

namespace BMM.UI.iOS.UI
{
    public class iOSUserDialogsFactory : IUserDialogsFactory
    {
        private readonly IViewModelAwareViewPresenter _viewModelAwareViewPresenter;

        public iOSUserDialogsFactory(IViewModelAwareViewPresenter viewModelAwareViewPresenter)
        {
            _viewModelAwareViewPresenter = viewModelAwareViewPresenter;
        }

        public IUserDialogs Create() => new iOSUserDialogs(_viewModelAwareViewPresenter);
    }
}