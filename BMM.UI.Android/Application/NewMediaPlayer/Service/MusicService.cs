using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using AndroidX.Media;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Security;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Constants.Player;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.Implementations.Notifications;
using BMM.UI.Droid.Application.NewMediaPlayer.Controller;
using BMM.UI.Droid.Application.NewMediaPlayer.Listeners;
using BMM.UI.Droid.Application.NewMediaPlayer.Notification;
using BMM.UI.Droid.Application.NewMediaPlayer.Playback;
using BMM.UI.Droid.Utils;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Plugin.Messenger;
using AudioAttributes = Com.Google.Android.Exoplayer2.Audio.AudioAttributes;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Service
{
    [Service(Name = "brunstad.MusicService")]
    public class MusicService : MediaBrowserServiceCompat, TimelineQueueEditor.IMediaDescriptionConverter
    {
        private const int DefaultBufferTimeInMs = 15000;
        private const int MinBufferTimeInMs = 300000;
        private const int MaxBufferTimeInMs = 300000;
        private NotificationManagerCompat _notificationManager;

        private NowPlayingNotificationBuilder _notificationBuilder;

        private MediaSessionCompat _mediaSession;

        private IExoPlayer _exoPlayer;
        private MediaSourceSetter _mediaSourceSetter;
        private MediaControllerCompat _mediaController;
        private bool _isForegroundService;

        private PeriodicExecutor _progressUpdater = new PeriodicExecutor();

        public static IExoPlayer CurrentExoPlayerInstance { get; private set; }
        
        private IExoPlayer ExoPlayer
        {
            get
            {
                if (_exoPlayer == null)
                {
                    var audioManager = (AudioManager)GetSystemService(AudioService);
                    
                    var playerInstance = new IExoPlayer.Builder(ApplicationContext)
                        !.SetTrackSelector(new DefaultTrackSelector(ApplicationContext))
                        !.SetMediaSourceFactory(_mediaSourceFactory)
                        !.SetLoadControl(new LoadControl())
                        !.Build();

                    playerInstance!.SetHandleAudioBecomingNoisy(true);
                    playerInstance.AddListener(new PlayerListener(playerInstance));

                    // ToDo: we actually allow Music and Speeches within one playlist. Now it's always music.
                    var audioAttributes = new AudioAttributes.Builder()
                        .SetContentType(C.ContentTypeMusic)
                        .SetUsage(C.UsageMedia)
                        .Build();
                    
                    playerInstance.SetAudioAttributes(audioAttributes, true);

                    _exoPlayer = playerInstance;
                    //_exoPlayer = new AudioFocusExoPlayerDecorator(playerInstance, audioManager, Mvx.IoCProvider.Resolve<ILogger>());
                    CurrentExoPlayerInstance = _exoPlayer;
                }

                return _exoPlayer;
            }
        }

        /// <summary>
        /// Triggers a periodic update of the current position and buffered position. Unfortunately MediaSessionConnector does not update
        /// the buffered position except when a status change happens. Therefore we have to do it in the MusicService. It might cause unforeseen
        /// problems and is not my preferred solution. Look in the history for an alternative implementation.
        /// </summary>
        private bool _periodicallyUpdateProgress;

        private SingleMediaSourceFactory _mediaSourceFactory;

        public bool PeriodicallyUpdateProgress
        {
            get => _periodicallyUpdateProgress;
            set
            {
                if (!_periodicallyUpdateProgress && value)
                {
                    _progressUpdater.SchedulePeriodicExecution(() =>
                    {
                        if (_exoPlayer == null)
                        {
                            // this should never happen in case it does I want to know about it
                            throw new Exception("_progressUpdater is called after destroying ExoPlayer");
                        }

                        if (Mvx.IoCProvider.CanResolve<IMvxMessenger>())
                        {
                            Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                                .ExecuteOnMainThreadAsync(() =>
                                    Mvx.IoCProvider.Resolve<IMvxMessenger>()
                                        .Publish(new PlaybackPositionChangedMessage(this)
                                        {
                                            CurrentPosition = ExoPlayer.CurrentPosition,
                                            BufferedPosition = ExoPlayer.BufferedPosition
                                        }));
                        }
                    });
                }
                if (!value)
                {
                    _progressUpdater.StopStateUpdater();
                }
                _periodicallyUpdateProgress = value;
            }
        }

        public override void OnCreate()
        {
            SetupHelper.EnsureInitialized();

            base.OnCreate();

            var sessionIntent = PackageManager.GetLaunchIntentForPackage(PackageName);
            var pendingIntent = PendingIntent.GetActivity(this, 0, sessionIntent, PendingIntentsUtils.GetImmutable());

            _mediaSession = new MediaSessionCompat(this, nameof(MusicService), null, pendingIntent);
            _mediaSession.Active = true;
            SessionToken = _mediaSession.SessionToken;
            
            _mediaController = new MediaControllerCompat(this, _mediaSession);
            _mediaController.RegisterCallback(
                new MusicServiceMediaCallback
                {
                    OnMetadataChangedImpl = compat => UpdateNotification(_mediaController.PlaybackState),
                    OnPlaybackStateChangedImpl = compat =>
                    {
                        UpdateNotification(compat);
                        PeriodicallyUpdateProgress = compat.IsPlaying();
                    }
                });
            
            var metadataMapper = Mvx.IoCProvider.Resolve<IMetadataMapper>();
            var queue = Mvx.IoCProvider.Resolve<IMediaQueue>();
            var analytics = Mvx.IoCProvider.Resolve<IAnalytics>();
            _notificationBuilder = new NowPlayingNotificationBuilder(this, metadataMapper, queue, Mvx.IoCProvider.Resolve<NotificationChannelBuilder>());
            _notificationManager = NotificationManagerCompat.From(this);
            
            _mediaSourceFactory = new SingleMediaSourceFactory(
                this,
                Mvx.IoCProvider.Resolve<IMediaRequestHttpHeaders>(),
                Mvx.IoCProvider.Resolve<IAccessTokenProvider>());
            
            var mediaSessionConnector = new MediaSessionConnector(_mediaSession);
            _mediaSourceSetter = new MediaSourceSetter(() => mediaSessionConnector,
                _ => new TimelineQueueEditor(_mediaController, new QueueDataAdapter(), this));
            
            var preparer = new ExoPlaybackPreparer(ExoPlayer, queue, metadataMapper, _mediaSourceFactory, _mediaSourceSetter, analytics);
            
            mediaSessionConnector.SetPlayer(ExoPlayer);
            mediaSessionConnector.SetPlaybackPreparer(preparer);
            mediaSessionConnector.SetQueueNavigator(new MetadataReadingQueueNavigator(_mediaSession, metadataMapper));
            //mediaSessionConnector.SetControlDispatcher(new IdleRecoveringControlDispatcher(_mediaSourceSetter));
            mediaSessionConnector.SetErrorMessageProvider(new CustomErrorMessageProvider(() => ExoPlayer,
                Mvx.IoCProvider.Resolve<ISdkVersionHelper>(),
                Mvx.IoCProvider.Resolve<ILogger>()));
            _mediaSourceSetter.CreateNew();
        }

        /// <summary>
        /// Handle case when user swipes the app away from the recent app list by stopping the service (and any ongoing playback).
        /// Unfortunately it's never triggered even though the documentation indicates it should: https://developer.android.com/reference/android/app/Service#onTaskRemoved(android.content.Intent)
        /// </summary>
        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);

            // By stopping playback, the player will transition to [Player.STATE_IDLE]. This will cause a state change in
            // the MediaSession, and (most importantly) call [MediaControllerCallback.onPlaybackStateChanged]. Because the
            // playback state will be reported as [PlaybackStateCompat.STATE_NONE], the service will first remove itself
            // as a foreground service, and will then call [stopSelf].
            ExoPlayer.Stop();
        }

        /// <summary>
        /// This is only called if the user kills the app (swipe from recent apps) while ExoPlayer is currently playing.
        /// </summary>
        public override void OnDestroy()
        {
            Mvx.IoCProvider.Resolve<IDownloadQueue>().AppWasKilled();
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new PlaybackStatusChangedMessage(this, new DefaultPlaybackState()));

            _mediaSession.Active = false;
            _mediaSession.Release();
            base.OnDestroy();
            _progressUpdater.Dispose();
            _exoPlayer.Stop();
            _exoPlayer = null;
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            // If we don't want to allow any arbitrary app to browser our content we need to check the origin

            return new BrowserRoot(ExoPlayerConstants.MediaIdRoot, null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            var list = new JavaList<MediaBrowserCompat.MediaItem>();
            // For now we always return an empty list since we don't really support browsing our library.
            // If we want to support Android Car or Android Wear we should return meaningful data.
            result.SendResult(list);
        }

        private void UpdateNotification(PlaybackStateCompat state)
        {
            if (string.IsNullOrEmpty(_mediaController.Metadata?.Description?.MediaId))
                return;

            Mvx.IoCProvider.Resolve<IExceptionHandler>()
                .FireAndForgetWithoutUserMessages(async () =>
                {
                    var updatedState = state.State;

                    Android.App.Notification notification = null;
                    if (updatedState != PlaybackStateCompat.StateNone)
                    {
                        notification = await _notificationBuilder.BuildNotification(_mediaSession.SessionToken, ApplicationContext, _mediaController);
                    }

                    if (updatedState == PlaybackStateCompat.StateBuffering || updatedState == PlaybackStateCompat.StatePlaying)
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.UpsideDownCake)
                            StartForeground(NowPlayingNotificationBuilder.NowPlayingNotification,
                                notification,
                                ForegroundService.TypeMediaPlayback);
                        else
                            StartForeground(NowPlayingNotificationBuilder.NowPlayingNotification, notification);
                        _isForegroundService = true;
                    }
                    else
                    {
                        if (_isForegroundService)
                        {

                            if (Build.VERSION.SdkInt >= BuildVersionCodes.UpsideDownCake)
                                StopForeground(StopForegroundFlags.Remove);
                            else
                                StopForeground(false);

                            if (notification != null)
                                _notificationManager.Notify(NowPlayingNotificationBuilder.NowPlayingNotification, notification);
                            else
                                RemoveNowPlayingNotification();

                            _isForegroundService = false;
                        }
                    }
                });
        }

        /// <summary>
        /// Removes the <see cref="NowPlayingNotificationBuilder.NowPlayingNotification"/> notification
        ///
        /// Since `stopForeground(false)` was already called (<see cref="UpdateNotification"/>)  it's possible to cancel the notification
        /// with `notificationManager.cancel(NOW_PLAYING_NOTIFICATION)` if minSdkVersion is >= [Build.VERSION_CODES.LOLLIPOP].
        /// 
        /// Prior to [Build.VERSION_CODES.LOLLIPOP], notifications associated with a foreground service remained marked as "ongoing" even
        /// after calling [Service.stopForeground], and cannot be cancelled normally.
        ///
        /// Fortunately, it's possible to simply call [Service.stopForeground] a second time, this time with `true`. This won't change
        /// anything about the service's state, but will simply remove the notification.
        /// </summary>
        private void RemoveNowPlayingNotification()
        {
            StopForeground(true);
        }

        private class LoadControl : DefaultLoadControl
        {
            public LoadControl() : base(new DefaultAllocator(true, C.DefaultBufferSegmentSize),
                MinBufferTimeInMs,
                MaxBufferTimeInMs,
                DefaultBufferTimeInMs,
                MinBufferTimeInMs,
                NumericConstants.Undefined,
                true,
                DefaultBackBufferDurationMs,
                DefaultRetainBackBufferFromKeyframe)
            {
            }
            
            /// <summary>
            /// Duration in microseconds of media to retain in the buffer prior to the current playback position, for fast backward seeking.
            /// </summary>
            public override long BackBufferDurationUs => C.MsToUs(System.Convert.ToInt64(TimeSpan.FromHours(1).TotalMilliseconds));
        }

        public MediaItem Convert(MediaDescriptionCompat description)
        {
            return Mvx.IoCProvider.Resolve<IMetadataMapper>().ToMediaItem(description);
        }
    }
}