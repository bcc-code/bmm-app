using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.LiveRadio;
using BMM.Core.NewMediaPlayer;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;

namespace BMM.Core.ViewModels
{
    public class ExploreRadioViewModel : BaseViewModel
    {
        private readonly ILiveClient _liveClient;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ILiveTime _liveTime;
        private readonly IPlayerErrorHandler _playerErrorHandler;

        private Timer _countdownTimer;
        private Timer _broadcastEndTimer;

        public IMvxCommand PlayCommand { get; }

        public bool ShowBmmLive => IsBroadcasting || IsBroadcastUpcoming;

        public bool IsBroadcasting => Track != null && (Track.RecordedAt < _liveTime.TimeOnServer);
        public bool IsBroadcastUpcoming => Track != null && !IsBroadcasting;

        public string Title => (IsBroadcastUpcoming ? TextSource[Translations.ExploreNewestViewModel_NextBroadcast] + " – " : "") + TextSource[Translations.ExploreNewestViewModel_BmmRadio];

        public TimeSpan? TimeLeft => Track != null && Track.RecordedAt != DateTime.MinValue ? Track.RecordedAt - _liveTime.TimeOnServer as TimeSpan? : null;

        private Track _liveTrack;
        public Track Track
        {
            get => _liveTrack;
            set => SetProperty(ref _liveTrack, value);
        }

        public ExploreRadioViewModel(ILiveClient liveClient, IExceptionHandler exceptionHandler, ILiveTime liveTime, IPlayerErrorHandler playerErrorHandler)
        {
            _liveClient = liveClient;
            _exceptionHandler = exceptionHandler;
            _liveTime = liveTime;
            _playerErrorHandler = playerErrorHandler;

            PlayCommand = new ExceptionHandlingCommand(() =>
            {
                if (IsBroadcasting)
                    return DocumentAction(Track, new List<Track> {Track});

                _playerErrorHandler.LiveRadioTooEarly();
                return Task.CompletedTask;
            });
        }

        public async Task Refresh()
        {
            RefreshSync();
        }

        private void RefreshSync()
        {
            _exceptionHandler.FireAndForget(async () => await LoadInfo());
        }

        public override async Task Initialize()
        {
            // Most of the times there is no live radio. Therefore it's supposed to load in the background and not make the ExploreNewestViewModel wait for the result of this.
            _exceptionHandler.FireAndForget(async () => await LoadInfo());
        }

        private async Task LoadInfo()
        {
            var liveInfo = await _liveClient.GetInfo();
            _liveTime.SetLiveInfo(liveInfo);
            Track = liveInfo.Track;

            // The order is important as to not mess up the scroll position
            await RaisePropertyChanged(() => IsBroadcastUpcoming);
            await RaisePropertyChanged(() => IsBroadcasting);
            await RaisePropertyChanged(() => ShowBmmLive);
            await RaisePropertyChanged(() => TimeLeft);
            await RaisePropertyChanged(() => Title);

            if (Track == null) return;

            if (TimeLeft.HasValue && TimeLeft.Value.TotalMinutes < 70) // We start the countdown 10 minutes early
            {
                _countdownTimer?.Dispose();
                _countdownTimer = new Timer(OnceASecond, null, 0, 1000);
            }

            StartBroadcastEndTimer();
        }

        private void OnceASecond(object state)
        {
            if (TimeLeft.HasValue && TimeLeft.Value.TotalSeconds < 0)
            {
                _countdownTimer?.Dispose();
                _countdownTimer = null;
                RaisePropertyChanged(() => IsBroadcasting);
                RaisePropertyChanged(() => IsBroadcastUpcoming);
                RaisePropertyChanged(() => ShowBmmLive);
                RaisePropertyChanged(() => Title);
            }
            else
            {
                RaisePropertyChanged(() => TimeLeft);
            }
        }

        private void StartBroadcastEndTimer()
        {
            var broadcastEnd = Track.RecordedAt.AddMilliseconds(Track.Duration);
            var msUntilBroadcastEnd = (int)broadcastEnd.Subtract(_liveTime.TimeOnServer).TotalMilliseconds;

            _broadcastEndTimer?.Dispose();
            _broadcastEndTimer = new Timer(OnceAtBroadcastEnd, null, msUntilBroadcastEnd, Timeout.Infinite);
        }

        private void OnceAtBroadcastEnd(object state)
        {
            RefreshSync();

            _broadcastEndTimer?.Dispose();
            _broadcastEndTimer = null;
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            _countdownTimer?.Dispose();
            _countdownTimer = null;
            _broadcastEndTimer?.Dispose();
            _broadcastEndTimer = null;
            base.ViewDestroy(viewFinishing);
        }
    }
}