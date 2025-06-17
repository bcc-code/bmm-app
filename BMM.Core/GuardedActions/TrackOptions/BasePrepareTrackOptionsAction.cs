using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.GuardedActions.TrackOptions.Interfaces;
using BMM.Core.GuardedActions.TrackOptions.Parameters;
using BMM.Core.GuardedActions.TrackOptions.Parameters.Interfaces;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.GuardedActions.Tracks.Parameters;
using BMM.Core.Helpers;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Tracks.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages;
using BMM.Core.Models.POs;
using BMM.Core.Models.TrackCollections;
using BMM.Core.Models.TrackCollections.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.NewMediaPlayer.Constants;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using Microsoft.Maui.Devices;

namespace BMM.Core.GuardedActions.TrackOptions
{
    public abstract class BasePrepareTrackOptionsAction :
        GuardedActionWithParameterAndResult<IPrepareTrackOptionsParameters, IList<StandardIconOptionPO>>,
        IPrepareTrackOptionsAction
    {
        private const string SleepTimerOptionKey = "Selected sleep timer option";
        private const string PlaybackSpeedOptionKey = "Selected playback speed option";
        private const string PlaybackSpeedStringFormat = "0.00";

        private readonly IConnection _connection;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IMvxMessenger _mvxMessenger;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly IShareLink _shareLink;
        private readonly ITrackOptionsService _trackOptionsService;
        private readonly IFeaturePreviewPermission _featurePreviewPermission;
        private readonly ISleepTimerService _sleepTimerService;
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
        private readonly IAnalytics _analytics;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IShowTrackInfoAction _showTrackInfoAction;
        private readonly ILikeUnlikeTrackAction _likeUnlikeTrackAction;
        private readonly IPlayNextAction _playNextAction;
        private readonly IAddToPlaylistAction _addToPlaylistAction;

        private readonly List<decimal> _availablePlaybackSpeed = new()
        {
            0.75m,
            1.0m,
            1.25m,
            1.5m
        };

        public BasePrepareTrackOptionsAction(IConnection connection,
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
            IAddToPlaylistAction addToPlaylistAction)
        {
            _connection = connection;
            _bmmLanguageBinder = bmmLanguageBinder;
            _mvxMessenger = mvxMessenger;
            _mvxNavigationService = mvxNavigationService;
            _shareLink = shareLink;
            _trackOptionsService = trackOptionsService;
            _featurePreviewPermission = featurePreviewPermission;
            _sleepTimerService = sleepTimerService;
            _firebaseRemoteConfig = firebaseRemoteConfig;
            _analytics = analytics;
            _mediaPlayer = mediaPlayer;
            _showTrackInfoAction = showTrackInfoAction;
            _likeUnlikeTrackAction = likeUnlikeTrackAction;
            _playNextAction = playNextAction;
            _addToPlaylistAction = addToPlaylistAction;
        }

        protected abstract string PlayNextIcon { get; }
        private bool IsSleepTimerOptionAvailable => _featurePreviewPermission.IsFeaturePreviewEnabled() || _firebaseRemoteConfig.IsSleepTimerEnabled;
        private bool IsPlaybackSpeedOptionAvailable => _featurePreviewPermission.IsFeaturePreviewEnabled() || _firebaseRemoteConfig.IsPlaybackSpeedEnabled;

        protected override async Task<IList<StandardIconOptionPO>> Execute(IPrepareTrackOptionsParameters parameter)
        {
            await Task.CompletedTask;
            var options = new List<StandardIconOptionPO>();

            var sourceVM = parameter.SourceVM;
            var track = (Track)parameter.Track;

            bool shouldShowSleepTimerOption = sourceVM is PlayerViewModel && IsSleepTimerOptionAvailable;
            bool shouldShowPlaybackSpeedOption = sourceVM is PlayerViewModel && IsPlaybackSpeedOptionAvailable;

            bool isInOnlineMode = _connection.GetStatus() == ConnectionStatus.Online;

            options.Add(new StandardIconOptionPO(
                track.IsLiked
                    ? _bmmLanguageBinder[Translations.Global_RemoveFromFavorites]
                    : _bmmLanguageBinder[Translations.Global_AddToFavorites], 
                track.IsLiked
                    ? ImageResourceNames.IconLiked
                    : ImageResourceNames.IconUnliked,
                new MvxAsyncCommand(() => _likeUnlikeTrackAction.ExecuteGuarded(new LikeOrUnlikeTrackActionParameter(track.IsLiked, track.Id)))));
            
            if (isInOnlineMode && !track.IsLivePlayback)
            {
                if (sourceVM is TrackCollectionViewModel trackCollectionVm)
                {
                    options.Add(
                        new StandardIconOptionPO(
                            _bmmLanguageBinder[Translations.UserDialogs_Track_DeleteFromPlaylist],
                            ImageResourceNames.IconRemove,
                            new MvxAsyncCommand(() => trackCollectionVm.DeleteTrackFromTrackCollection(track))));
                }

                options.Add(
                    new StandardIconOptionPO(
                        _bmmLanguageBinder[Translations.UserDialogs_Track_AddToPlaylist],
                        ImageResourceNames.IconPlaylist,
                        new MvxAsyncCommand(() => _addToPlaylistAction.ExecuteGuarded(new TrackActionsParameter(track, sourceVM.PlaybackOriginString())))));
            }

            if (sourceVM is not QueueViewModel && sourceVM is not PlayerViewModel)
            {
                var mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();

                options.Add(new StandardIconOptionPO(
                    _bmmLanguageBinder[Translations.UserDialogs_Track_QueueToPlayNext],
                    PlayNextIcon,
                    new MvxAsyncCommand(() => _playNextAction.ExecuteGuarded(
                        new TrackActionsParameter(track, sourceVM.PlaybackOriginString())))));

                options.Add(
                    new StandardIconOptionPO(
                        _bmmLanguageBinder[Translations.UserDialogs_Track_AddToQueue],
                        ImageResourceNames.IconQueue,
                        new MvxAsyncCommand(async () =>
                        {
                            var success = await mediaPlayer.AddToEndOfQueue(track, sourceVM.PlaybackOriginString());
                            if (success)
                            {
                                await Mvx.IoCProvider.Resolve<IToastDisplayer>()
                                    .Success(_bmmLanguageBinder.GetText(Translations.UserDialogs_Track_AddedToQueue, track.Title));

                                _analytics
                                    .LogEvent(Event.TrackHasBeenAddedToEndOfQueue,
                                        new Dictionary<string, object>
                                        {
                                            { "track", track.Id }
                                        });
                            }
                        })));
            }

            // Only show this option if we are not inside an album (what apparently is the one you would be guided to here)
            if (sourceVM is not AlbumViewModel && isInOnlineMode && track.ParentId != 0)
            {
                options.Add(
                    new StandardIconOptionPO(
                        _bmmLanguageBinder[Translations.UserDialogs_Track_GoToAlbum],
                        ImageResourceNames.IconAlbum,
                        new MvxAsyncCommand(async () =>
                        {
                            await ClosePlayer();
                            await _mvxNavigationService.Navigate<AlbumViewModel, Album>(new Album
                                { Id = track.ParentId, Title = track.Album });
                        })));
            }

            if (isInOnlineMode)
            {
                IList<TrackRelation> contributors = track.Relations?.Where(r =>
                        r.Type == TrackRelationType.Composer ||
                        r.Type == TrackRelationType.Interpret ||
                        r.Type == TrackRelationType.Lyricist ||
                        r.Type == TrackRelationType.Arranger)
                    .ToList();

                if (contributors != null && contributors.Count > 0)
                {
                    var trackHasOnlyOneContributor = contributors.Count == 1;

                    options.Add(
                        new StandardIconOptionPO(_bmmLanguageBinder[Translations.UserDialogs_Track_GoToContributor],
                            ImageResourceNames.IconPerson,
                            new MvxAsyncCommand(async () =>
                            {
                                var goToContributorOptions = new List<StandardIconOptionPO>();

                                foreach (TrackRelation relation in track.Relations)
                                {
                                    if (relation.Type == TrackRelationType.Composer)
                                    {
                                        var rel = (TrackRelationComposer)relation;

                                        if (trackHasOnlyOneContributor)
                                            await GoToContributorVM(rel.Id);
                                        else
                                        {
                                            goToContributorOptions.Add(new StandardIconOptionPO(
                                                _bmmLanguageBinder.GetText(Translations.UserDialogs_Track_GoToContributor_Composer,
                                                    rel.Name),
                                                ImageResourceNames.IconPerson,
                                                new MvxAsyncCommand(() => GoToContributorVM(rel.Id))));
                                        }
                                    }
                                    else if (relation.Type == TrackRelationType.Interpret)
                                    {
                                        var rel = (TrackRelationInterpreter)relation;

                                        if (trackHasOnlyOneContributor)
                                            await GoToContributorVM(rel.Id);
                                        else
                                        {
                                            goToContributorOptions.Add(new StandardIconOptionPO(
                                                _bmmLanguageBinder.GetText(Translations.UserDialogs_Track_GoToContributor_Interpret,
                                                    rel.Name),
                                                ImageResourceNames.IconPerson,
                                                new MvxAsyncCommand(() => GoToContributorVM(rel.Id))));
                                        }
                                    }
                                    else if (relation.Type == TrackRelationType.Lyricist)
                                    {
                                        var rel = (TrackRelationLyricist)relation;

                                        if (trackHasOnlyOneContributor)
                                            await GoToContributorVM(rel.Id);
                                        else
                                        {
                                            goToContributorOptions.Add(new StandardIconOptionPO(
                                                _bmmLanguageBinder.GetText(Translations.UserDialogs_Track_GoToContributor_Lyricist,
                                                    rel.Name),
                                                ImageResourceNames.IconPerson,
                                                new MvxAsyncCommand(() => GoToContributorVM(rel.Id))));
                                        }
                                    }
                                    else if (relation.Type == TrackRelationType.Arranger)
                                    {
                                        var rel = (TrackRelationArranger)relation;

                                        if (trackHasOnlyOneContributor)
                                            await GoToContributorVM(rel.Id);
                                        else
                                        {
                                            goToContributorOptions.Add(new StandardIconOptionPO(
                                                _bmmLanguageBinder.GetText(Translations.UserDialogs_Track_GoToContributor_Arranger,
                                                    rel.Name),
                                                ImageResourceNames.IconPerson,
                                                new MvxAsyncCommand(() => GoToContributorVM(rel.Id))));
                                        }
                                    }
                                }

                                if (goToContributorOptions.Any())
                                    await _trackOptionsService.OpenOptions(goToContributorOptions);
                            })));
                }
            }

            options.AddIf(() => shouldShowPlaybackSpeedOption,
                new StandardIconOptionPO(
                    _bmmLanguageBinder[Translations.PlayerViewModel_PlaybackSpeed],
                    ImageResourceNames.IconPlaybackSpeed,
                    new MvxAsyncCommand(async () => { await PlaybackSpeedClickedAction(); })));

            options.AddIf(() => shouldShowSleepTimerOption,
                new StandardIconOptionPO(
                    _bmmLanguageBinder[Translations.PlayerViewModel_SleepTimer],
                    ImageResourceNames.IconSleepTimer,
                    new MvxAsyncCommand(async () => await SleepTimerClickedAction())));

            options.AddIfNotNull(GetTranscriptionOption(track));
            
            options.Add(
                new StandardIconOptionPO(
                    _bmmLanguageBinder[Translations.UserDialogs_Track_MoreInformation],
                    ImageResourceNames.IconInfo,
                    new MvxAsyncCommand(async () =>
                    {
                        await _showTrackInfoAction.ExecuteGuarded(track);
                    })));
            
            options.AddIf(() => sourceVM is QueueViewModel,
                new StandardIconOptionPO(
                    _bmmLanguageBinder[Translations.QueueViewModel_RemoveFromQueueOption],
                    ImageResourceNames.IconRemove,
                    new MvxAsyncCommand(() => _mediaPlayer.DeleteFromQueue(track))));

            if (sourceVM is PlayerBaseViewModel)
            {
                long currentPosition = _mediaPlayer.CurrentPosition;
                options.Add(new StandardIconOptionPO(
                        _bmmLanguageBinder.GetText(Translations.UserDialogs_Track_ShareFrom, GetFormattedCurrentPosition(currentPosition)),
                        ImageResourceNames.IconShare,
                        new MvxAsyncCommand(async () => { await _shareLink.Share(track, currentPosition); })));
            }

            options.Add(new StandardIconOptionPO(
                    _bmmLanguageBinder[Translations.UserDialogs_Track_Share],
                    ImageResourceNames.IconShare,
                    new MvxAsyncCommand(async () => { await _shareLink.Share(track); })));

            return options;
        }

        private string GetFormattedCurrentPosition(long currentPosition)
        {
            var converter = new MillisecondsToTimeValueConverter();
            return (string)converter.Convert(currentPosition, typeof(string), null, CultureInfo.CurrentCulture);
        }

        private StandardIconOptionPO GetTranscriptionOption(Track track)
        {
            if (!track.HasTranscription)
                return default;

            string optionName = track.IsSong()
                ? _bmmLanguageBinder[Translations.ReadTranscriptionViewModel_Lyrics]
                : _bmmLanguageBinder[Translations.ReadTranscriptionViewModel_Transcription];

            return new StandardIconOptionPO(optionName,
                ImageResourceNames.IconInfo,
                new MvxAsyncCommand(() =>
                {
                    return _mvxNavigationService.Navigate<ReadTranscriptionViewModel, TranscriptionParameter>(
                        new TranscriptionParameter(track));
                }));
        }

        private async Task PlaybackSpeedClickedAction()
        {
            IMvxCommand CreatePlaybackSpeedTapCommand(decimal playbackSpeed)
            {
                return new MvxCommand(() =>
                {
                    _mediaPlayer.ChangePlaybackSpeed(playbackSpeed);
                    _analytics.LogEvent(Event.PlaybackSpeedChanged, new Dictionary<string, object>
                    {
                        {PlaybackSpeedOptionKey, playbackSpeed}
                    });
                });
            }

            StandardIconOptionPO CreatePlaybackSpeedOption(decimal playbackSpeed)
            {
                string optionName = playbackSpeed == PlayerConstants.NormalPlaybackSpeed
                    ? _bmmLanguageBinder[Translations.PlayerViewModel_Normal]
                    : $"{playbackSpeed.ToString(PlaybackSpeedStringFormat, CultureInfo.InvariantCulture)}x";

                return new StandardIconOptionPO(
                    GetPlaybackSpeedOptionTitle(optionName, playbackSpeed),
                    ImageResourceNames.IconPlaybackSpeed,
                    CreatePlaybackSpeedTapCommand(playbackSpeed));
            }

            var playbackSpeedOptions = _availablePlaybackSpeed
                .Select(CreatePlaybackSpeedOption)
                .ToList();

            _analytics.LogEvent(Event.PlaybackSpeedOptionsOpened);
            await _trackOptionsService.OpenOptions(playbackSpeedOptions);
        }

        private string GetPlaybackSpeedOptionTitle(string optionName, decimal speed)
        {
            var titleStringBuilder = new StringBuilder(optionName);

            if (IsCurrentPlaybackSpeed(speed))
                titleStringBuilder.Append($" ({_bmmLanguageBinder.GetText(Translations.PlayerViewModel_Selected)})");

            return titleStringBuilder.ToString();
        }

        private bool IsCurrentPlaybackSpeed(decimal playbackSpeed)
        {
            return _mediaPlayer.CurrentPlaybackSpeed == playbackSpeed;
        }

        private async Task SleepTimerClickedAction()
        {
            IMvxCommand CreateOptionTapCommand(SleepTimerOption sleepTimerOption)
            {
                return new MvxCommand(() =>
                {
                    if (sleepTimerOption == SleepTimerOption.NotSet)
                    {
                        _sleepTimerService.Disable();
                        return;
                    }

                    _sleepTimerService.Set(sleepTimerOption);
                    _analytics.LogEvent(Event.SleepTimerOptionSelected, new Dictionary<string, object>
                    {
                        {SleepTimerOptionKey, sleepTimerOption.ToString()}
                    });
                });
            }

            StandardIconOptionPO PrepareMinutesOption(SleepTimerOption sleepTimerOption)
            {
                return new StandardIconOptionPO(
                    GetSleepTimerOptionTitle(_bmmLanguageBinder.GetText(Translations.PlayerViewModel_Minutes, (int)sleepTimerOption), sleepTimerOption),
                    ImageResourceNames.IconSleepTimer,
                    CreateOptionTapCommand(sleepTimerOption));
            }

            var sleepTimerOptions = new List<StandardIconOptionPO>
            {
                PrepareMinutesOption(SleepTimerOption.FiveMinutes),
                PrepareMinutesOption(SleepTimerOption.TenMinutes),
                PrepareMinutesOption(SleepTimerOption.FifteenMinutes),
                PrepareMinutesOption(SleepTimerOption.ThirtyMinutes),
                PrepareMinutesOption(SleepTimerOption.FortyFiveMinutes),
                new(
                    GetSleepTimerOptionTitle(_bmmLanguageBinder.GetText(Translations.PlayerViewModel_Hour, 1), SleepTimerOption.OneHour),
                    ImageResourceNames.IconSleepTimer,
                    CreateOptionTapCommand(SleepTimerOption.OneHour))
            };

            if (_sleepTimerService.IsEnabled)
            {
                sleepTimerOptions.Add(new StandardIconOptionPO(
                    _bmmLanguageBinder.GetText(Translations.PlayerViewModel_Disable),
                    ImageResourceNames.IconRemove,
                    CreateOptionTapCommand(SleepTimerOption.NotSet)));
            }

            _analytics.LogEvent(Event.SleepTimerOptionsOpened);
            await _trackOptionsService.OpenOptions(sleepTimerOptions);
        }

        private string GetSleepTimerOptionTitle(string optionName, SleepTimerOption sleepTimerOption)
        {
            var titleStringBuilder = new StringBuilder(optionName);

            if (IsCurrentSleepTimerOption(sleepTimerOption))
                titleStringBuilder.Append($" ({_bmmLanguageBinder.GetText(Translations.PlayerViewModel_Selected)})");

            return titleStringBuilder.ToString();
        }

        private bool IsCurrentSleepTimerOption(SleepTimerOption sleepTimerOption)
        {
            return _sleepTimerService.CurrentSleepTimerOption == sleepTimerOption;
        }

        private async Task GoToContributorVM(int id)
        {
            await ClosePlayer();
            await _mvxNavigationService.Navigate<ContributorViewModel, int>(id);
        }

        private async Task ClosePlayer()
        {
            await _mvxNavigationService.ChangePresentation(new CloseFragmentsOverPlayerHint());
            _mvxMessenger.Publish(new TogglePlayerMessage(this, false));
            await Task.Delay(ViewConstants.DefaultAnimationDurationInMilliseconds);
        }
    }
}
