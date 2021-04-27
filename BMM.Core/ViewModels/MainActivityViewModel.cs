using BMM.Core.Messages;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class MainActivityViewModel : MvxViewModel
    {
        private readonly IMvxMessenger _messenger;
        private readonly IMvxNavigationService _navigationService;
        private MvxSubscriptionToken _logoutToken;

        public MainActivityViewModel(IMvxMessenger messenger, IMvxNavigationService navigationService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _logoutToken = _messenger.Subscribe<LoggedOutMessage>(UserLoggedOut);
        }

        private void UserLoggedOut(LoggedOutMessage obj)
        {
            _navigationService.Close(this);
        }
    }
}