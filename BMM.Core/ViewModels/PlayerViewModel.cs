using System;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Implementations;
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
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.ViewModels
{
    public sealed class PlayerViewModel : PlayerBaseViewModel, IPlayerViewModel, IMvxViewModel<bool>
    {
        private readonly IUriOpener _uriOpener;
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
        public MvxCommand OpenLyricsCommand { get; }
        
        public string SongTreasureLink { get; set; }

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

        public bool CanNavigateToLanguageChange => NavigateToLanguageChangeCommand.CanExecute();
        
        public bool HasLyrics => OpenLyricsCommand.CanExecute();

        public string PlayingText => _queueLength > 0 ? TextSource.GetText(Translations.PlayerViewModel_PlayingCount, _currentIndex + 1, _queueLength) : string.Empty;

        public PlayerViewModel(
            IMediaPlayer mediaPlayer,
            IUriOpener uriOpener,
            IChangeTrackLanguageAction changeTrackLanguageAction,
            IUpdateExternalRelationsAction updateExternalRelationsAction) : base(mediaPlayer)
        {
            _uriOpener = uriOpener;
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
            OpenLyricsCommand = new MvxCommand(OpenLyricsLink, () => !string.IsNullOrEmpty(SongTreasureLink));

            _repeatToken = Messenger.Subscribe<RepeatModeChangedMessage>(m => RepeatType = m.RepeatType);
            _shuffleToken = Messenger.Subscribe<ShuffleModeChangedMessage>(m => IsShuffleEnabled = m.IsShuffleEnabled);

            // read current values
            RepeatType = MediaPlayer.RepeatType;
            IsShuffleEnabled = MediaPlayer.IsShuffleEnabled;
            UpdatePlaybackState(MediaPlayer.PlaybackState);
            SetupSubscriptions();
        }

        public override ITrackInfoProvider TrackInfoProvider => _playerTrackInfoProvider ??= new PlayerTrackInfoProvider();

        private void OpenLyricsLink() => _uriOpener.OpenUri(new Uri(SongTreasureLink));

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
            Messenger.Unsubscribe<TogglePlayerMessage>(_toggleToken);
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

        // Since CurrentTrack is a ITrackModel, this method always gets a null value passed in from Mvx. Therefore we override it and pass CurrentTrack directly.
        protected override Task ShowTrackInfo(Track track)
        {
            return base.ShowTrackInfo(CurrentTrack as Track);
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