using Android.OS;
using Android.Support.V4.Media.Session;
using BMM.Api.Abstraction;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.NewMediaPlayer.Service;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using Uri = Android.Net.Uri;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    public class ExoPlaybackPreparer : Java.Lang.Object, MediaSessionConnector.IPlaybackPreparer
    {
        public const string MediaIdShuffle = "shuffle";
        private readonly IExoPlayer _exoPlayer;

        private readonly IMediaQueue _mediaQueue;
        private readonly IMetadataMapper _metadataMapper;
        private readonly IMediaSource.IFactory _mediaSourceFactory;
        private readonly MediaSourceSetter _mediaSource;
        private readonly IAnalytics _analytics;

        public ExoPlaybackPreparer(IExoPlayer exoPlayer, IMediaQueue mediaQueue, IMetadataMapper metadataMapper, IMediaSource.IFactory mediaSourceFactory,
            MediaSourceSetter mediaSource, IAnalytics analytics)
        {
            _exoPlayer = exoPlayer;
            _mediaQueue = mediaQueue;
            _metadataMapper = metadataMapper;
            _mediaSourceFactory = mediaSourceFactory;
            _mediaSource = mediaSource;
            _analytics = analytics;
        }

        public void OnPrepareFromMediaId(string mediaIdString, bool playWhenReady, Bundle extras)
        {
            var trackList = _mediaQueue.Tracks;
            int? startIndex;
            var startTimeInMs = extras.GetLong("startTimeInMs");

            if (mediaIdString == MediaIdShuffle)
            {
                startIndex = new Random().Next(trackList.Count);
                _exoPlayer.ShuffleModeEnabled = true;
            }
            else
            {
                var mediaId = int.Parse(mediaIdString);
                startIndex = trackList.FindIndex(t => t.Id == mediaId);
            }

            var mediaSources = new List<IMediaSource>();
            foreach (var mediaTrack in trackList)
            {
                try
                {
                    mediaSources.Add(GetSingleMediaSource(mediaTrack));
                }
                catch (Exception ex)
                {
                    _analytics.LogEvent("Exception when adding track to queue",
                        new Dictionary<string, object> {{"id", mediaTrack.Id}, {"language", mediaTrack.Language}, {"exception", ex}});
                }
            }

            _mediaSource.CreateNew(startIndex.GetValueOrDefault(), mediaSources.ToArray());

            _exoPlayer.SetMediaSource(_mediaSource.Get);
            _exoPlayer.Prepare();
            _exoPlayer.SeekTo(startIndex.Value, startTimeInMs);

            SetPlayWhenReadyOnPlayer(playWhenReady);
        }

        private IMediaSource GetSingleMediaSource(IMediaTrack track)
        {
            var description = _metadataMapper.FromTrack(track);
            return _mediaSourceFactory.CreateMediaSource(_metadataMapper.ToMediaItem(description));
        }

        /// <summary>
        /// Since <see cref="MediaSessionConnector.OnPrepareFromMediaId"/> no longer calls <see cref="IExoPlayer.PlayWhenReady"/> have to do it ourselves
        /// Old version <see href="https://github.com/google/ExoPlayer/blob/r2.9.6/extensions/mediasession/src/main/java/com/google/android/exoplayer2/ext/mediasession/MediaSessionConnector.java"/>
        /// New version <see href="https://github.com/google/ExoPlayer/blob/release-v2/extensions/mediasession/src/main/java/com/google/android/exoplayer2/ext/mediasession/MediaSessionConnector.java" />
        /// </summary>
        private void SetPlayWhenReadyOnPlayer(bool playWhenReady)
        {
            _exoPlayer.PlayWhenReady = playWhenReady;
        }

        public long SupportedPrepareActions => PlaybackStateCompat.ActionPrepareFromMediaId | PlaybackStateCompat.ActionPlayFromMediaId;

        #region Unsupported use cases

        /// <summary>
        /// Handles callbacks to both [MediaSessionCompat.Callback.onPrepareFromSearch]
        /// *AND* [MediaSessionCompat.Callback.onPlayFromSearch] when using [MediaSessionConnector].
        /// This method is used by the Google Assistant to respond to requests such as:
        /// - Play Geisha from Wake Up on BMM
        /// - Play electronic music on BMM
        /// - Play music on BMM
        /// </summary>
        public void OnPrepareFromSearch(string query, bool playWhenReady, Bundle extras)
        {
        }

        public void OnPrepareFromUri(Uri uri, bool playWhenReady, Bundle extras)
        {
        }

        public bool OnCommand(IPlayer player, string command, Bundle extras, ResultReceiver cb)
        {
            return false;
        }
        
        public void OnPrepare(bool playWhenReady)
        {
        }

        #endregion


    }
}