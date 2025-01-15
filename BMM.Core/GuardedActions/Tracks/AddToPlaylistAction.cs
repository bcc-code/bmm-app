using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.GuardedActions.Tracks.Parameters;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Messages;
using BMM.Core.ViewModels;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.GuardedActions.Tracks;

public class AddToPlaylistAction
    : GuardedActionWithParameter<TrackActionsParameter>,
      IAddToPlaylistAction
{
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly IMvxMessenger _mvxMessenger;

    public AddToPlaylistAction(
        IMvxNavigationService mvxNavigationService,
        IMvxMessenger mvxMessenger)
    {
        _mvxNavigationService = mvxNavigationService;
        _mvxMessenger = mvxMessenger;
    }
    
    protected override async Task Execute(TrackActionsParameter parameter)
    {
        await _mvxNavigationService.ChangePresentation(new CloseFragmentsOverPlayerHint());
        _mvxMessenger.Publish(new TogglePlayerMessage(this, false));
        await Task.Delay(ViewConstants.DefaultAnimationDurationInMilliseconds);

        await _mvxNavigationService.Navigate<TrackCollectionsAddToViewModel, TrackCollectionsAddToViewModel.Parameter>(
            new TrackCollectionsAddToViewModel.Parameter
            {
                DocumentId = parameter.Track.Id,
                DocumentType = parameter.Track.DocumentType,
                OriginViewModel = parameter.PlaybackOrigin
            });
    }
}