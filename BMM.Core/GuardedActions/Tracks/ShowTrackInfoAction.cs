using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Messages;
using BMM.Core.ViewModels;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.GuardedActions.Tracks
{
    public class ShowTrackInfoAction
        : GuardedActionWithParameter<Track>,
          IShowTrackInfoAction
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly IMvxMessenger _mvxMessenger;

        public ShowTrackInfoAction(IMvxNavigationService navigationService, IMvxMessenger mvxMessenger)
        {
            _navigationService = navigationService;
            _mvxMessenger = mvxMessenger;
        }
        
        protected override async Task Execute(Track track)
        {
            await _navigationService.ChangePresentation(new CloseFragmentsOverPlayerHint());
            _mvxMessenger.Publish(new TogglePlayerMessage(this, false));
            await Task.Delay(ViewConstants.DefaultAnimationDurationInMilliseconds);
            await _navigationService.Navigate<TrackInfoViewModel, Track>(track);
        }
    }
}