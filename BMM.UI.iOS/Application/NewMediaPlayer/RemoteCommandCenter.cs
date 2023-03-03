using System;
using BMM.Api.Abstraction;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using FFImageLoading;
using Foundation;
using MediaPlayer;
using MvvmCross.Base;
using UIKit;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public interface ICommandCenter : IMediaPlayerInitializer
    {
        void PlaybackStateChanged(IPlaybackState state, ITrackModel currentTrack);
    }

    public class RemoteCommandCenter : ICommandCenter
    {
        private readonly IMvxMainThreadAsyncDispatcher _dispatcher;
        private readonly IExceptionHandler _exceptionHandler;

        public RemoteCommandCenter(IMvxMainThreadAsyncDispatcher dispatcher, IExceptionHandler exceptionHandler)
        {
            _dispatcher = dispatcher;
            _exceptionHandler = exceptionHandler;
        }

        public void PlaybackStateChanged(IPlaybackState state, ITrackModel currentTrack)
        {
            if (currentTrack == null)
            {
                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = null;
                return;
            }

            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                var info = new MPNowPlayingInfo
                {
                    Title = currentTrack.Title,
                    AlbumTitle = currentTrack.Album,
                    Artist = currentTrack.Artist,
                    DefaultPlaybackRate = 1.0,
                    PlaybackRate = state.PlaybackRate,
                    ElapsedPlaybackTime = state.CurrentPosition / 1000,
                    PlaybackQueueCount = state.QueueLength,
                    PlaybackQueueIndex = Convert.ToInt32(state.CurrentIndex),
                    MediaType = MPNowPlayingInfoMediaType.Audio,
                    IsLiveStream = currentTrack.IsLivePlayback,
                    CurrentPlaybackDate = NSDate.Now
                };

                if (currentTrack.Duration > 0)
                {
                    info.PlaybackDuration = currentTrack.Duration / 1000;
                    info.PlaybackProgress = state.CurrentPosition / currentTrack.Duration;
                }

                var coverImage = currentTrack.ArtworkUri == null
                    ? await ImageService.Instance.LoadCompiledResource("placeholder_cover").AsUIImageAsync()
                    : await ImageService.Instance.LoadUrl(currentTrack.ArtworkUri).AsUIImageAsync();
                
                info.Artwork = new MPMediaItemArtwork(coverImage);

                if (state.PlayStatus != PlayStatus.Playing)
                {
                    info.PlaybackRate = 0.0;
                }


                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = info;
            });
        }

        public virtual void Initialize()
        {
            _dispatcher.ExecuteOnMainThreadAsync(() =>
            {
                UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            });
        }

        public virtual void Deinitialize()
        {
            _dispatcher.ExecuteOnMainThreadAsync(() =>
            {
                UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
            });
        }
    }
}