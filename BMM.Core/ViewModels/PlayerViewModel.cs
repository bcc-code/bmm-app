﻿using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Interactions;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.ViewModels;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.Enums;
using BMM.Core.Models.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters;

namespace BMM.Core.ViewModels
{
    public sealed class PlayerViewModel : PlayerBaseViewModel, IPlayerViewModel, IMvxViewModel<bool>
    {
        private readonly IUriOpener _uriOpener;
        private readonly IAnalytics _analytics;
        private readonly IUpdateExternalRelationsAction _updateExternalRelationsAction;
        private readonly MvxInteraction<TogglePlayerInteraction> _closePlayerInteraction = new();
        private RepeatType _repeatType;
        private bool _isShuffleEnabled;
        private bool _isSkipToNextEnabled;
        private bool _isSkipToPreviousEnabled;
        private string _trackLanguage;
        private long _currentIndex;
        private int _queueLength;
        
        private MvxSubscriptionToken _toggleToken;
        private MvxSubscriptionToken _repeatToken;
        private MvxSubscriptionToken _shuffleToken;
        
        private bool _directlyShowPlayerForAndroid;
        private bool _hasExternalRelations;
        private PlayerTrackInfoProvider _playerTrackInfoProvider;
        private PlayerLeftButtonType? _leftButtonType;
        private bool _hasTranscription;
        private string _watchBccMediaLink;

        public IMvxInteraction<TogglePlayerInteraction> ClosePlayerInteraction => _closePlayerInteraction;

        public MvxCommand CloseViewModelCommand { get; }
        public MvxCommand ClosePlayerCommand { get; }
        public IMvxCommand OpenQueueCommand { get; }
        public IMvxCommand ToggleShuffleCommand { get; }
        public IMvxCommand ToggleRepeatCommand { get; }
        public IMvxAsyncCommand NavigateToLanguageChangeCommand { get; }
        public MvxCommand PreviousOrSeekToStartCommand { get; }
        public MvxCommand PreviousCommand { get; }
        public MvxCommand NextCommand { get; }
        public MvxCommand SkipForwardCommand { get; }
        public MvxCommand SkipBackwardCommand { get; }
        public MvxCommand LeftButtonClickedCommand { get; }
        public MvxCommand WatchButtonClickedCommand { get; }
        public string LeftButtonLink { get; set; }

        public bool IsShuffleEnabled
        {
            get => _isShuffleEnabled;
            private set => SetProperty(ref _isShuffleEnabled, value);
        }

        public RepeatType RepeatType
        {
            get => _repeatType;
            private set => SetProperty(ref _repeatType, value);
        }

        public bool IsSkipToNextEnabled
        {
            get => _isSkipToNextEnabled;
            private set
            {
                SetProperty(ref _isSkipToNextEnabled, value);
                NextCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsSkipToPreviousEnabled
        {
            get => _isSkipToPreviousEnabled;
            private set
            {
                SetProperty(ref _isSkipToPreviousEnabled, value);
                PreviousCommand.RaiseCanExecuteChanged();
                PreviousOrSeekToStartCommand.RaiseCanExecuteChanged();
            }
        }
        
        public bool HasExternalRelations
        {
            get => _hasExternalRelations;
            set => SetProperty(ref _hasExternalRelations, value);
        }
        
        public string TrackLanguage
        {
            get => _trackLanguage;
            set => SetProperty(ref _trackLanguage, value);
        }

        public PlayerLeftButtonType? LeftButtonType
        {
            get => _leftButtonType;
            set => SetProperty(ref _leftButtonType, value);
        }

        public bool HasTranscription
        {
            get => _hasTranscription;
            set => SetProperty(ref _hasTranscription, value);
        }

        public string WatchBccMediaLink
        {
            get => _watchBccMediaLink;
            set
            {
                SetProperty(ref _watchBccMediaLink, value);
                WatchButtonClickedCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanNavigateToLanguageChange => NavigateToLanguageChangeCommand.CanExecute();
        
        public bool HasLeftButton => LeftButtonClickedCommand.CanExecute();
        public bool HasWatchButton => WatchButtonClickedCommand.CanExecute();

        public string PlayingText => _queueLength > 0 ? TextSource.GetText(Translations.PlayerViewModel_PlayingCount, _currentIndex + 1, _queueLength) : string.Empty;

        public PlayerViewModel(
            IMediaPlayer mediaPlayer,
            IUriOpener uriOpener,
            IAnalytics analytics,
            IChangeTrackLanguageAction changeTrackLanguageAction,
            IUpdateExternalRelationsAction updateExternalRelationsAction,
            IShowTrackInfoAction showTrackInfoAction,
            ILikeUnlikeTrackAction likeUnlikeTrackAction) : base(mediaPlayer)
        {
            _uriOpener = uriOpener;
            _analytics = analytics;
            _updateExternalRelationsAction = updateExternalRelationsAction;

            _updateExternalRelationsAction.AttachDataContext(this);
            changeTrackLanguageAction.AttachDataContext(this);

            NavigateToLanguageChangeCommand = changeTrackLanguageAction.Command;
            CloseViewModelCommand = new MvxCommand(() => NavigationService.Close(this));
            ClosePlayerCommand = new MvxCommand(() => _closePlayerInteraction.Raise(new TogglePlayerInteraction {Open = false}));
            OpenQueueCommand = NavigationService.NavigateCommand<QueueViewModel>();
            ToggleRepeatCommand = new MvxCommand(() => MediaPlayer.ToggleRepeatType());
            ToggleShuffleCommand = new MvxCommand(() => MediaPlayer.ToggleShuffle());
            NextCommand = new MvxCommand(MediaPlayer.PlayNext, () => IsSkipToNextEnabled);
            PreviousCommand = new MvxCommand(MediaPlayer.PlayPrevious, () => IsSkipToPreviousEnabled);
            PreviousOrSeekToStartCommand = new MvxCommand(MediaPlayer.PlayPreviousOrSeekToStart, () => IsSkipToPreviousEnabled);
            SkipForwardCommand = new MvxCommand(() => MediaPlayer.JumpForward());
            SkipBackwardCommand = new MvxCommand(() => MediaPlayer.JumpBackward());
            LeftButtonClickedCommand = new MvxCommand(LeftButtonClicked, () => HasTranscription || !string.IsNullOrEmpty(LeftButtonLink));
            WatchButtonClickedCommand = new MvxCommand(BccMediaClicked, () => !string.IsNullOrEmpty(WatchBccMediaLink));
            
            SeekToPositionCommand = new MvxCommand<long>(position =>
            {
                MediaPlayer.SeekTo(position);
            });
            ShowTrackInfoCommand = new MvxCommand(() => showTrackInfoAction.Command.ExecuteAsync((Track)CurrentTrack));
            LikeUnlikeCommand = new MvxAsyncCommand(async () =>
            {
                await likeUnlikeTrackAction.ExecuteGuarded(new LikeOrUnlikeTrackActionParameter(IsLiked, CurrentTrack.Id));
                await RaisePropertyChanged(() => IsLiked);
            });
            
            _repeatToken = Messenger.Subscribe<RepeatModeChangedMessage>(m => RepeatType = m.RepeatType);
            _shuffleToken = Messenger.Subscribe<ShuffleModeChangedMessage>(m => IsShuffleEnabled = m.IsShuffleEnabled);

            // read current values
            RepeatType = MediaPlayer.RepeatType;
            IsShuffleEnabled = MediaPlayer.IsShuffleEnabled;
            UpdatePlaybackState(MediaPlayer.PlaybackState);
        }

        public override ITrackInfoProvider TrackInfoProvider => _playerTrackInfoProvider ??= new PlayerTrackInfoProvider();
        public IMvxCommand<long> SeekToPositionCommand { get; set; }
        public IMvxCommand ShowTrackInfoCommand { get; set; }
        public IMvxCommand LikeUnlikeCommand { get; set; }
        
        private async void LeftButtonClicked()
        {
            if (HasTranscription)
            {
                if (CurrentTrack == null)
                    return;

                await NavigationService.Navigate<ReadTranscriptionViewModel, TranscriptionParameter>(
                    new TranscriptionParameter(CurrentTrack));
                return;
            }
            
            _uriOpener.OpenUri(new Uri(LeftButtonLink));
            _analytics.LogEvent(LeftButtonType == PlayerLeftButtonType.Lyrics
                ? Event.LyricsOpened
                : Event.BCCMediaOpenedFromPlayer);
        }
        
        private void BccMediaClicked()
        {
            _uriOpener.OpenUri(new Uri(WatchBccMediaLink));
            _analytics.LogEvent(Event.BCCMediaOpenedFromPlayer);
        }

        public void Prepare(bool showPlayer)
        {
            _directlyShowPlayerForAndroid = showPlayer;
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            _toggleToken = Messenger.SubscribeOnMainThread<TogglePlayerMessage>(OnTogglePlayerMessage);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            if (_directlyShowPlayerForAndroid)
                _closePlayerInteraction.Raise(new TogglePlayerInteraction {Open = true});
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            Messenger.UnsubscribeSafe<TogglePlayerMessage>(_toggleToken);
        }

        protected override async Task OptionsAction(Document item)
        {
            if (CurrentTrack == null)
                return;

            var queue = Mvx.IoCProvider.Resolve<IMediaQueue>();
            var currentDocument = queue.Tracks.FirstOrDefault(track => track.Id == CurrentTrack.Id) as Document;

            if (currentDocument != null)
                await base.OptionsAction(currentDocument);
        }

        private void OnTogglePlayerMessage(TogglePlayerMessage message)
        {
            _closePlayerInteraction.Raise(new TogglePlayerInteraction {Open = message.Open});
        }

        protected override void UpdatePlaybackState(IPlaybackState state)
        {
            base.UpdatePlaybackState(state);

            IsSkipToNextEnabled = state.IsSkipToNextEnabled;
            IsSkipToPreviousEnabled = state.IsSkipToPreviousEnabled;

            _currentIndex = state.CurrentIndex;
            _queueLength = state.QueueLength;
            RaisePropertyChanged(() => PlayingText);
        }

        protected override async Task OnCurrentTrackChanged()
        {
            await base.OnCurrentTrackChanged();

            await _updateExternalRelationsAction.ExecuteGuarded();
            await RaisePropertyChanged(() => CanNavigateToLanguageChange);
            NavigateToLanguageChangeCommand.RaiseCanExecuteChanged();
        }
    }
}