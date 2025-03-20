using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Messages;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
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
        private readonly IFirebaseRemoteConfig _remoteConfig;

        public ShowTrackInfoAction(IMvxNavigationService navigationService, IMvxMessenger mvxMessenger, IFirebaseRemoteConfig remoteConfig)
        {
            _navigationService = navigationService;
            _mvxMessenger = mvxMessenger;
            _remoteConfig = remoteConfig;
        }
        
        protected override async Task Execute(Track track)
        {
            await ClosePlayer();
            
            if (_remoteConfig.ContainsDailyPodcastTag(track.Tags))
            {
                await _navigationService.Navigate<BibleStudyViewModel, IBibleStudyParameters>(new BibleStudyParameters(track));
                return;
            }

            await _navigationService.Navigate<TrackInfoViewModel, Track>(track);
        }
        
        private async Task ClosePlayer()
        {
            await _navigationService.ChangePresentation(new CloseFragmentsOverPlayerHint());
            _mvxMessenger.Publish(new TogglePlayerMessage(this, false));
            await Task.Delay(ViewConstants.DefaultAnimationDurationInMilliseconds);
        }
    }
}