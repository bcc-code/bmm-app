using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Api.Utils;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Implementations;
using BMM.Core.Interactions;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.ViewModels;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using BMM.Core.Implementations.UI;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Translation;
using BMM.Core.Utils;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.ViewModels
{
    public sealed class PlayerViewModel : PlayerBaseViewModel, IPlayerViewModel, IMvxViewModel<bool>
    {
        private MvxSubscriptionToken _toggleToken;
        private MvxSubscriptionToken _repeatToken;
        private MvxSubscriptionToken _shuffleToken;
        
        private string _songTreasureLink;

        private readonly IUriOpener _uriOpener;
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;

        private readonly MvxInteraction<TogglePlayerInteraction> _closePlayerInteraction = new MvxInteraction<TogglePlayerInteraction>();

        private bool _directlyShowPlayerForAndroid;
        private bool _hasExternalRelations;
        
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

        private bool _isShuffleEnabled;
        public bool IsShuffleEnabled
        {
            get => _isShuffleEnabled;
            private set => SetProperty(ref _isShuffleEnabled, value);
        }

        private RepeatType _repeatType = RepeatType.None;
        public RepeatType RepeatType
        {
            get => _repeatType;
            private set => SetProperty(ref _repeatType, value);
        }

        private bool _isSkipToNextEnabled;
        public bool IsSkipToNextEnabled
        {
            get => _isSkipToNextEnabled;
            private set
            {
                SetProperty(ref _isSkipToNextEnabled, value);
                NextCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isSkipToPreviousEnabled;
        private string _trackLanguage;

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
            private set => SetProperty(ref _hasExternalRelations, value);
        }
        
        public string TrackLanguage
        {
            get => _trackLanguage;
            private set => SetProperty(ref _trackLanguage, value);
        }

        public bool CanNavigateToLanguageChange => NavigateToLanguageChangeCommand.CanExecute();
        
        public bool HasLyrics => OpenLyricsCommand.CanExecute();

        private long CurrentIndex { get; set; }

        private int QueueLength { get; set; }

        public string PlayingText => QueueLength > 0 ? TextSource.GetText(Translations.PlayerViewModel_PlayingCount, CurrentIndex + 1, QueueLength) : string.Empty;

        public PlayerViewModel(
            IMediaPlayer mediaPlayer,
            IUriOpener uriOpener,
            IFirebaseRemoteConfig firebaseRemoteConfig,
            IChangeTrackLanguageAction changeTrackLanguageAction) : base(mediaPlayer)
        {
            _uriOpener = uriOpener;
            _firebaseRemoteConfig = firebaseRemoteConfig;
                
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
            OpenLyricsCommand = new MvxCommand(OpenLyricsLink, () => !string.IsNullOrEmpty(_songTreasureLink));

            _repeatToken = Messenger.Subscribe<RepeatModeChangedMessage>(m => RepeatType = m.RepeatType);
            _shuffleToken = Messenger.Subscribe<ShuffleModeChangedMessage>(m => IsShuffleEnabled = m.IsShuffleEnabled);

            // read current values
            RepeatType = MediaPlayer.RepeatType;
            IsShuffleEnabled = MediaPlayer.IsShuffleEnabled;
            UpdatePlaybackState(MediaPlayer.PlaybackState);
            UpdateExternalRelations();
            SetupSubscriptions();
        }

        private void OpenLyricsLink() => _uriOpener.OpenUri(new Uri(_songTreasureLink));

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
            {
                await base.OptionsAction(currentDocument);
            }
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

            CurrentIndex = state.CurrentIndex;
            QueueLength = state.QueueLength;
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

            UpdateExternalRelations();
            await RaisePropertyChanged(() => CanNavigateToLanguageChange);
            NavigateToLanguageChangeCommand.RaiseCanExecuteChanged();
        }

        private void UpdateExternalRelations()
        {
            try
            {
                if (CurrentTrack == null)
                {
                    _songTreasureLink = string.Empty;
                    return;
                }

                TrackLanguage = new CultureInfo(CurrentTrack.Language).NativeName;
            
                HasExternalRelations = CurrentTrack?.Relations != null &&
                                       CurrentTrack.Relations.Any(relation => relation.Type == TrackRelationType.External);
            
                var existingSongbook = CurrentTrack
                    ?.Relations
                    ?.OfType<TrackRelationSongbook>()
                    .FirstOrDefault();

                if (existingSongbook != null)
                {
                    _songTreasureLink = string.Format(_firebaseRemoteConfig.SongTreasuresSongLink,
                        SongbookUtils.GetShortName(existingSongbook.Name),
                        existingSongbook.Id);
                }
                else
                {
                    _songTreasureLink = string.Empty;
                }
            }
            finally
            {
                RaisePropertyChanged(() => HasLyrics);
                OpenLyricsCommand.RaiseCanExecuteChanged();
            }
        }
    }
}