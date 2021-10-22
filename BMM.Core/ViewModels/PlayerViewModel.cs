using System;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
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
using BMM.Core.Translation;

namespace BMM.Core.ViewModels
{
    public sealed class PlayerViewModel : PlayerBaseViewModel, IMvxViewModel<bool>
    {
        private MvxSubscriptionToken _toggleToken;
        private MvxSubscriptionToken _repeatToken;
        private MvxSubscriptionToken _shuffleToken;

        private readonly IUriOpener _uriOpener;
        private readonly IExceptionHandler _exceptionHandler;

        private readonly MvxInteraction<TogglePlayerInteraction> _closePlayerInteraction = new MvxInteraction<TogglePlayerInteraction>();

        private bool _directlyShowPlayerForAndroid;

        public IMvxInteraction<TogglePlayerInteraction> ClosePlayerInteraction => _closePlayerInteraction;

        public MvxCommand CloseViewModelCommand { get; }

        public MvxCommand ClosePlayerCommand { get; }

        public IMvxCommand OpenQueueCommand { get; }

        public IMvxCommand ToggleShuffleCommand { get; }

        public IMvxCommand ToggleRepeatCommand { get; }

        public MvxCommand PreviousOrSeekToStartCommand { get; }

        public MvxCommand PreviousCommand { get; }

        public MvxCommand NextCommand { get; }

        public MvxCommand SkipForwardCommand { get; }

        public MvxCommand SkipBackwardCommand { get; }

        public MvxCommand OpenExternalReferenceCommand { get; }

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

        private bool _hasExternalRelations;
        public bool HasExternalRelations
        {
            get => _hasExternalRelations;
            private set => SetProperty(ref _hasExternalRelations, value);
        }

        private Uri _btvLink;
        public string BtvLinkTitle { get; private set; }
        public bool HasBtvLink => _btvLink != null;

        private long CurrentIndex { get; set; }

        private int QueueLength { get; set; }

        public string PlayingText => QueueLength > 0 ? TextSource.GetText(Translations.PlayerViewModel_PlayingCount, CurrentIndex + 1, QueueLength) : string.Empty;

        public PlayerViewModel(IMediaPlayer mediaPlayer, IUriOpener uriOpener, IExceptionHandler exceptionHandler) : base(mediaPlayer)
        {
            _uriOpener = uriOpener;
            _exceptionHandler = exceptionHandler;

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
            OpenExternalReferenceCommand = new MvxCommand(() => OpenBtvLink());

            _repeatToken = Messenger.Subscribe<RepeatModeChangedMessage>(m => RepeatType = m.RepeatType);
            _shuffleToken = Messenger.Subscribe<ShuffleModeChangedMessage>(m => IsShuffleEnabled = m.IsShuffleEnabled);

            // read current values
            RepeatType = MediaPlayer.RepeatType;
            IsShuffleEnabled = MediaPlayer.IsShuffleEnabled;
            UpdatePlaybackState(MediaPlayer.PlaybackState);
            UpdateExternalRelations();
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
            {
                _closePlayerInteraction.Raise(new TogglePlayerInteraction {Open = true});
            }
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

        private void OpenBtvLink()
        {
            try
            {
                _uriOpener.OpenUri(_btvLink);
            }
            catch (FormatException ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }

        protected override async Task OnCurrentTrackChanged(CurrentTrackChangedMessage message)
        {
            await base.OnCurrentTrackChanged(message);

            UpdateExternalRelations();
        }

        private void UpdateExternalRelations()
        {
            HasExternalRelations = CurrentTrack?.Relations != null &&
                                   CurrentTrack.Relations.Any(relation => relation.Type == TrackRelationType.External);

            _btvLink = null;
            BtvLinkTitle = null;
            if (HasExternalRelations)
            {
                var btvHost = new Uri("https://brunstad.tv/").Host;
                foreach (var link in CurrentTrack.Relations.OfType<TrackRelationExternal>())
                {
                    if (!Uri.TryCreate(link.Url, UriKind.Absolute, out var uri))
                        continue;
                    if (uri.Host == btvHost)
                    {
                        _btvLink = uri;
                        BtvLinkTitle = link.Name;
                        break;
                    }
                }
            }

            RaisePropertyChanged(() => HasBtvLink);
            RaisePropertyChanged(() => BtvLinkTitle);
        }
    }
}