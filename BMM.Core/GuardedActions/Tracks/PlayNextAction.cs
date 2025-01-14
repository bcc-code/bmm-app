using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.GuardedActions.Tracks.Parameters;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;

namespace BMM.Core.GuardedActions.Tracks;

public class PlayNextAction
    : GuardedActionWithParameter<TrackActionsParameter>,
      IPlayNextAction
{
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IToastDisplayer _toastDisplayer;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IAnalytics _analytics;

    public PlayNextAction(
        IMediaPlayer mediaPlayer,
        IToastDisplayer toastDisplayer,
        IBMMLanguageBinder bmmLanguageBinder,
        IAnalytics analytics)
    {
        _mediaPlayer = mediaPlayer;
        _toastDisplayer = toastDisplayer;
        _bmmLanguageBinder = bmmLanguageBinder;
        _analytics = analytics;
    }
    
    protected override async Task Execute(TrackActionsParameter parameter)
    {
        bool success = await _mediaPlayer.QueueToPlayNext(parameter.Track, parameter.PlaybackOrigin);

        if (success)
        {
            await _toastDisplayer
                .Success(_bmmLanguageBinder.GetText(Translations.UserDialogs_Track_AddedToQueue, parameter.Track.Title));

            _analytics
                .LogEvent(Event.TrackHasBeenAddedToBePlayedNext,
                    new Dictionary<string, object>
                    {
                        { "track", parameter.Track.Id }
                    });
        }
    }
}