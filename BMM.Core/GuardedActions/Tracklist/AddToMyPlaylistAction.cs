using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Core.ExceptionHandlers.Interfaces;
using BMM.Core.ExceptionHandlers.Interfaces.Base;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.Messages;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.GuardedActions.Tracklist
{
    public class AddToMyPlaylistAction
        : GuardedAction,
          IAddToMyPlaylistAction
    {
        private readonly IBMMClient _bmmClient;
        private readonly IMvxMessenger _mvxMessenger;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly IFollowOwnTrackCollectionExceptionHandler _followOwnTrackCollectionExceptionHandler;

        public AddToMyPlaylistAction(
            IBMMClient bmmClient,
            IMvxMessenger mvxMessenger,
            IMvxNavigationService mvxNavigationService,
            IFollowOwnTrackCollectionExceptionHandler followOwnTrackCollectionExceptionHandler)
        {
            _bmmClient = bmmClient;
            _mvxMessenger = mvxMessenger;
            _mvxNavigationService = mvxNavigationService;
            _followOwnTrackCollectionExceptionHandler = followOwnTrackCollectionExceptionHandler;
        }

        private ISharedTrackCollectionViewModel SharedTrackCollectionViewModel => this.GetDataContext();

        protected override async Task Execute()
        {
            int id = SharedTrackCollectionViewModel.MyCollection.Id;
            string name = SharedTrackCollectionViewModel.MyCollection.Name;

            await _bmmClient.SharedPlaylist.Follow(SharedTrackCollectionViewModel.SharingSecret);
            _mvxMessenger.Publish(new PlaylistStateChangedMessage(this, SharedTrackCollectionViewModel.MyCollection.Id));
            await SharedTrackCollectionViewModel.CloseCommand.ExecuteAsync();
            await _mvxNavigationService.Navigate<TrackCollectionViewModel, ITrackCollectionParameter>(
                new TrackCollectionParameter(id, name));
        }

        protected override IEnumerable<IActionExceptionHandler> GetExceptionHandlers()
        {
            yield return _followOwnTrackCollectionExceptionHandler;
        }
    }
}