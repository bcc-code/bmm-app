﻿using AVFoundation;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.NewMediaPlayer.Constants;
using BMM.Core.Utils;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class IosMediaPlayer : IPlatformSpecificMediaPlayer, IDisposable
    {
        private readonly TimeSpan _timeToWaitBeforePreloadingNextItem = TimeSpan.FromSeconds(8);
        private long SeekToStartThresholdInMs = 5000;

        private readonly IShuffleableQueue _queue;
        private readonly IAudioPlayback _audioPlayback;
        private readonly IMvxMessenger _messenger;
        private readonly ILogger _logger;
        private readonly ICommandCenter _commandCenter;
        private readonly NSObject _avAudioSessionInterruptionNotification;
        private readonly IAnalytics _analytics;
        private readonly IExceptionHandler _exceptionHandler;

        private static IMediaTrack _currentTrack;
        private static int _currentTrackIndex;
        private readonly DebounceDispatcher _debounceDispatcher;

        public IosMediaPlayer(
            IAudioPlayback audioPlayback,
            IShuffleableQueue queue,
            IMvxMessenger messenger,
            ILogger logger,
            ICommandCenter commandCenter,
            IExceptionHandler exceptionHandler,
            IAnalytics analytics)
        {
            _audioPlayback = audioPlayback;
            _queue = queue;
            _messenger = messenger;
            _logger = logger;
            _commandCenter = commandCenter;
            _exceptionHandler = exceptionHandler;
            _analytics = analytics;

            _debounceDispatcher = new DebounceDispatcher((int)_timeToWaitBeforePreloadingNextItem.TotalMilliseconds);

            _audioPlayback.OnMediaFinished = OnMediaFinished;
            _audioPlayback.OnPositionChanged = PlaybackPositionChanged;
            _audioPlayback.OnStateChanged = PlaybackStateChanged;
            _avAudioSessionInterruptionNotification = AVAudioSession.Notifications.ObserveInterruption((sender, args) =>
            {
                if (args.InterruptionType != AVAudioSessionInterruptionType.Ended ||
                    args.Option != AVAudioSessionInterruptionOptions.ShouldResume ||
                    _audioPlayback.Status == PlayStatus.Stopped ||
                    _audioPlayback.Status == PlayStatus.Ended) return;
                exceptionHandler.HandleException(_audioPlayback.Play());
                PlaybackStateChanged();
            });
        }

        public Action ContinuingPreviousSession { get; set; }

        private void OnMediaFinished()
        {
            var trackCompletedMessage = new TrackCompletedMessage(this);

            if (_queue.RepeatMode == RepeatType.RepeatOne || _queue.RepeatMode == RepeatType.RepeatAll && _queue.Tracks.Count == 1)
            {
                _messenger.Publish(trackCompletedMessage);
                SeekTo(0);
                if (_queue.RepeatMode == RepeatType.RepeatOne)
                    _analytics.LogEvent("repeated single track");
            }
            else if (GetPlayNextIndex().HasValue)
            {
                _messenger.Publish(trackCompletedMessage);
                PlayNext();
            }
            else
            {
                trackCompletedMessage.PlayStatus = PlayStatus.Ended;
                trackCompletedMessage.NumberOfTracksInQueue = _queue.Tracks.Count;
                _messenger.Publish(trackCompletedMessage);
                PlaybackStateChanged();
            }
        }

        public ITrackModel CurrentTrack => _currentTrack;

        public bool IsPlaying => _audioPlayback.Status == PlayStatus.Buffering || _audioPlayback.Status == PlayStatus.Loading || _audioPlayback.Status == PlayStatus.Playing;

        public RepeatType RepeatType => _queue.RepeatMode;

        public bool IsShuffleEnabled => _queue.IsShuffleEnabled;

        public long CurrentPosition => _audioPlayback.Position;

        public IPlaybackState PlaybackState => new PlaybackState
        {
            IsPlaying = IsPlaying,
            PlayStatus = _audioPlayback.Status,
            PlaybackRate = _audioPlayback.Rate,
            DesiredPlaybackRate = _audioPlayback.DesiredRate,
            IsSkipToNextEnabled = GetPlayNextIndex().HasValue,
            IsSkipToPreviousEnabled = CurrentTrack?.IsLivePlayback == false || GetPlayPreviousIndex().HasValue,
            CurrentIndex = _currentTrackIndex,
            QueueLength = _queue.Tracks.Count,
            CurrentPosition = _audioPlayback.Position,
            BufferedPosition = _audioPlayback.Buffered
        };

        public async Task Play(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, long startTimeInMs = 0, bool resetPlaybackSpeed = true)
        {
            bool result = true;

            if (!_queue.IsSameQueue(mediaTracks))
                result = await _queue.Replace(mediaTracks, currentTrack);

            if (result || CurrentTrack == null)
            {
                int index = mediaTracks.IndexOf(currentTrack);
                PlayTrack(currentTrack, index, result, startTimeInMs).FireAndForget();
            }
            
            if (resetPlaybackSpeed)
                ChangePlaybackSpeed(PlayerConstants.NormalPlaybackSpeed);
        }

        public async Task RecoverQueue(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, long startTimeInMs = 0)
        {
            _currentTrack = currentTrack;
            _currentTrackIndex = mediaTracks.IndexOf(currentTrack);

            await _queue.Replace(mediaTracks, currentTrack);
            await _audioPlayback.SetPlayerTrack(currentTrack);
            WaitAndPreloadNextTrack();

            if (startTimeInMs > 0)
                _audioPlayback.SeekTo(startTimeInMs, false);

            PlaybackStateChanged();
            _messenger.Publish(new CurrentTrackChangedMessage(currentTrack, startTimeInMs, this));
        }

        private void WaitAndPreloadNextTrack()
        {
            _debounceDispatcher.Run(() =>
            {
                _exceptionHandler.FireAndForgetWithoutUserMessages(async () => await PreloadNextTrackIfNeeded());
            });
        }

        private async Task PreloadNextTrackIfNeeded()
        {
            int? nextTrackIndex = GetPlayNextIndex();
            if (nextTrackIndex == null)
                return;

            await _audioPlayback.PreloadTrack(_queue.Tracks[nextTrackIndex.Value]);
        }

        public void PlayPause()
        {
            if (_audioPlayback.Status == PlayStatus.Ended)
            {
                // After the queue has been completed start again at the first track
                PlayTrackByIndex(0).FireAndForget();
            }
            else
            {
                _audioPlayback.PlayPause();
                PlaybackStateChanged();
            }
        }

        public void ChangePlaybackSpeed(decimal playbackSpeed) => _audioPlayback.DesiredRate = playbackSpeed;

        public decimal CurrentPlaybackSpeed => _audioPlayback.DesiredRate;
        
        private int? GetPlayNextIndex()
        {
            if (_currentTrackIndex < _queue.Tracks.Count - 1)
            {
                return _currentTrackIndex + 1;
            }

            if (_queue.RepeatMode == RepeatType.RepeatAll)
            {
                return 0;
            }

            return null;
        }

        public void PlayNext()
        {
            var newIndex = GetPlayNextIndex();

            if (newIndex.HasValue)
            {
                PlayTrackByIndex(newIndex.Value).FireAndForget();
            }
            else
            {
                _logger.Warn("IosMediaPlayer", "It should not be possible to play next track");
            }
        }

        private int? GetPlayPreviousIndex()
        {
            if (_currentTrackIndex > 0)
            {
                return _currentTrackIndex - 1;
            }

            if (_queue.RepeatMode == RepeatType.RepeatAll)
            {
                return _queue.Tracks.Count - 1;
            }

            return null;
        }

        public void PlayPrevious()
        {
            var newIndex = GetPlayPreviousIndex();

            if (newIndex.HasValue)
            {
                PlayTrackByIndex(newIndex.Value).FireAndForget();
            }
            else
            {
                SeekTo(0);
            }
        }

        public void PlayPreviousOrSeekToStart()
        {
            if (_audioPlayback.Position <= SeekToStartThresholdInMs || CurrentTrack.IsLivePlayback)
            {
                PlayPrevious();
            }
            else
            {
                SeekTo(0);
            }
        }

        public async Task DeleteFromQueue(IMediaTrack track)
        {
            await Task.CompletedTask;
            _queue.Delete(track);
            SetCurrentTrackIndex();
        }

        public Task<bool> AddToEndOfQueue(IMediaTrack track, string playbackOrigin, bool ignoreIfAlreadyAdded = false)
        {
            return _queue.Append(track);
        }

        public Task<bool> QueueToPlayNext(IMediaTrack track, string playbackOrigin)
        {
            return _queue.PlayNext(track, _currentTrack);
        }

        public void SetRepeatType(RepeatType type)
        {
            _queue.SetRepeatType(type);
            _messenger.Publish(new RepeatModeChangedMessage(this) { RepeatType = type });
        }

        public void SetShuffle(bool isShuffleEnabled)
        {
            _queue.SetShuffle(isShuffleEnabled, _currentTrack);
            SetCurrentTrackIndex();

            _messenger.Publish(new ShuffleModeChangedMessage(this) {IsShuffleEnabled = isShuffleEnabled});
            PlaybackStateChanged();
        }

        private void SetCurrentTrackIndex()
        {
            _currentTrackIndex = _queue.Tracks.IndexOf(_currentTrack);
        }

        public void SeekTo(long newPosition)
        {
            _messenger.Publish(new PlaybackSeekedMessage(this)
            {
                CurrentPosition = _audioPlayback.Position,
                SeekedPosition = newPosition
            });

            _audioPlayback.SeekTo(newPosition);
        }

        public async Task ShuffleList(IList<IMediaTrack> tracks, string playbackOrigin)
        {
            var randomIndex = new Random().Next(tracks.Count);
            var track = tracks[randomIndex];
            await Play(tracks, track);
            if (_queue.Tracks.Any(t => t.Id == track.Id))
                SetShuffle(true);
        }

        private async Task PlayTrackByIndex(int index)
        {
            await PlayTrack(_queue.Tracks[index], index, true);
        }

        private async Task PlayTrack(IMediaTrack track, int index, bool shouldPlay, long startTimeInMs = 0)
        {
            _messenger.Publish(new PlaybackSeekedMessage(this)
            {
                CurrentPosition = (PlaybackState.PlayStatus == PlayStatus.Ended) ? CurrentTrack.Duration : _audioPlayback.Position,
                SeekedPosition = startTimeInMs
            });

            _currentTrack = track;
            _currentTrackIndex = index;

            if (shouldPlay) // Even if the user is online we should not play a not-downloaded track from Downloaded Content
            {
                await _audioPlayback.Play(_currentTrack);
                _audioPlayback.SeekTo(startTimeInMs);
            }

            PlaybackStateChanged();
            _messenger.Publish(new CurrentTrackChangedMessage(track, startTimeInMs, this));
            WaitAndPreloadNextTrack();
        }

        private void PlaybackStateChanged()
        {
            var message = new PlaybackStatusChangedMessage(this, PlaybackState);

            // command center can't just use IMvxMessenger because those messages don't get delivered if the app is not in focus
            _commandCenter.PlaybackStateChanged(message.PlaybackState, _currentTrack);
            _messenger.Publish(message);
        }

        private void PlaybackPositionChanged(long position, long bufferedPosition)
        {
            _messenger.Publish(new PlaybackPositionChangedMessage(this)
            {
                CurrentPosition = position,
                BufferedPosition = bufferedPosition
            });
        }

        public void Stop()
        {
            _currentTrack = null;
            _currentTrackIndex = 0;
            _audioPlayback.Stop();
            _messenger.Publish(new CurrentTrackChangedMessage(null, 0, this));
        }

        public void Dispose()
        {
            _avAudioSessionInterruptionNotification.Dispose();
        }
    }
}