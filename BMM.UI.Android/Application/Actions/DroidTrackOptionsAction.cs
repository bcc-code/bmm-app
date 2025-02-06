using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.GuardedActions.TrackOptions;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.Droid.Application.Actions;

public class DroidTrackOptionsAction : BasePrepareTrackOptionsAction
{
    public DroidTrackOptionsAction(IConnection connection,
        IBMMLanguageBinder bmmLanguageBinder,
        IMvxMessenger mvxMessenger,
        IMvxNavigationService mvxNavigationService,
        IShareLink shareLink,
        ITrackOptionsService trackOptionsService,
        IFeaturePreviewPermission featurePreviewPermission,
        ISleepTimerService sleepTimerService,
        IFirebaseRemoteConfig firebaseRemoteConfig,
        IAnalytics analytics,
        IMediaPlayer mediaPlayer,
        IShowTrackInfoAction showTrackInfoAction,
        ILikeUnlikeTrackAction likeUnlikeTrackAction,
        IPlayNextAction playNextAction,
        IAddToPlaylistAction addToPlaylistAction) : base(connection,
        bmmLanguageBinder,
        mvxMessenger,
        mvxNavigationService,
        shareLink,
        trackOptionsService,
        featurePreviewPermission,
        sleepTimerService,
        firebaseRemoteConfig,
        analytics,
        mediaPlayer,
        showTrackInfoAction,
        likeUnlikeTrackAction,
        playNextAction,
        addToPlaylistAction)
    {
    }

    protected override string PlayNextIcon => ImageResourceNames.IconPlayNext;
}