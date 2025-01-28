using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using AndroidX.Media.Session;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Activities;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Implementations.Notifications;
using BMM.UI.Droid.Utils;
using FFImageLoading;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public class NowPlayingNotificationBuilder
    {
        public const string ActionSkipBackward = "org.brunstad.bmm.android.skipbackward";
        public const string ActionSkipForward = "org.brunstad.bmm.android.skipforward";
        public const string JumpForwardTitle = "Jump forward";
        public const string JumpBackwardTitle = "Jump back";

        private readonly Context _context;
        private readonly IMetadataMapper _metadataMapper;
        private readonly IMediaQueue _queue;
        private readonly NotificationChannelBuilder _channelBuilder;

        private readonly NotificationManager _notificationManager;

        private readonly ExoPlayerAction _skipToPreviousAction;
        private readonly INotificationAction _jumpBackwardAction;
        private readonly INotificationAction _jumpForwardAction;
        private readonly ExoPlayerAction _playAction;
        private readonly ExoPlayerAction _pauseAction;
        private readonly ExoPlayerAction _skipToNextAction;

        private readonly PendingIntent _stopPendingIntent;

        public NowPlayingNotificationBuilder(Context context, IMetadataMapper metadataMapper, IMediaQueue queue, NotificationChannelBuilder channelBuilder)
        {
            _context = context;
            _metadataMapper = metadataMapper;
            _queue = queue;
            _channelBuilder = channelBuilder;
            _notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

            _skipToPreviousAction = new ExoPlayerAction(Resource.Drawable.icon_prev_notification, "Previous", PlaybackStateCompat.ActionSkipToPrevious);
            _jumpBackwardAction = new CustomAction(Resource.Drawable.icon_skip_back_notification, JumpBackwardTitle, ActionSkipBackward);
            _jumpForwardAction = new CustomAction(Resource.Drawable.icon_skip_forward_notification, JumpForwardTitle, ActionSkipForward);
            _playAction = new ExoPlayerAction(Resource.Drawable.icon_play_notification, "Play", PlaybackStateCompat.ActionPlay);
            _pauseAction = new ExoPlayerAction(Resource.Drawable.icon_pause_notification, "Pause", PlaybackStateCompat.ActionPause);
            //ToDo: when skipping a song it should be registered in PlayStatistics
            _skipToNextAction = new ExoPlayerAction(Resource.Drawable.icon_next_notification, "Next", PlaybackStateCompat.ActionSkipToNext);
            _stopPendingIntent = MediaButtonIntent(PlaybackStateCompat.ActionStop);
        }

        public async Task<Android.App.Notification> BuildNotification(MediaSessionCompat.Token sessionToken, Context applicationContext, MediaControllerCompat controller)
        {
            _channelBuilder.EnsureChannel(_notificationManager,
                NowPlayingChannel,
                () => new NotificationChannel(NowPlayingChannel, "Now playing", NotificationImportance.Low)
                {
                    Description = "Shows what music is currently playing in BMM"
                });

            var description = controller.Metadata.Description;
            var playbackState = controller.PlaybackState;

            var builder = new NotificationCompat.Builder(_context, NowPlayingChannel);
            var lastActionIndex = 0;

            var actionsInCompactView = new List<int>();

            var track = _metadataMapper.LookupTrackFromMetadata(controller.Metadata, _queue);
            var displayJumpActions = (playbackState.IsPlaying() || playbackState.IsPlayEnabled() || playbackState.IsSkipToNextEnabled()) && track?.IsLivePlayback == false;

            if (playbackState.IsSkipToPreviousEnabled())
            {
                builder.AddAction(_skipToPreviousAction, _context);
                lastActionIndex++;
            }

            if (displayJumpActions)
            {
                builder.AddAction(_jumpBackwardAction, _context);
                actionsInCompactView.Add(lastActionIndex++);
            }

            if (playbackState.IsPlaying())
            {
                builder.AddAction(_pauseAction, _context);
                actionsInCompactView.Add(lastActionIndex++);
                long startTime = Java.Lang.JavaSystem.CurrentTimeMillis() - controller.PlaybackState.Position;
                builder.SetWhen(startTime);
                builder.SetUsesChronometer(true);
            }
            else if (playbackState.IsPlayEnabled())
            {
                builder.AddAction(_playAction, _context);
                actionsInCompactView.Add(lastActionIndex++);
            }

            if (displayJumpActions)
            {
                builder.AddAction(_jumpForwardAction, _context);
                lastActionIndex++;
            }

            if (playbackState.IsSkipToNextEnabled())
            {
                builder.AddAction(_skipToNextAction, _context);
                actionsInCompactView.Add(lastActionIndex);
            }

            var mediaStyle = new AndroidX.Media.App.NotificationCompat.MediaStyle()
                .SetCancelButtonIntent(_stopPendingIntent)
                .SetMediaSession(sessionToken)
                .SetShowActionsInCompactView(actionsInCompactView.ToArray())
                .SetShowCancelButton(true);

            var notificationIntent = new Intent(applicationContext, typeof(MainActivity));
            var contentIntent = PendingIntent.GetActivity(applicationContext, 0, notificationIntent, PendingIntentsUtils.GetImmutable());

            builder = builder.SetContentIntent(contentIntent)
                .SetContentText(description.Subtitle)
                .SetContentTitle(description.Title)
                .SetDeleteIntent(_stopPendingIntent)
                .SetOnlyAlertOnce(true)
                .SetSmallIcon(Resource.Drawable.xam_mediaManager_notify_ic)
                .SetStyle(mediaStyle)
                .SetVisibility(NotificationCompat.VisibilityPublic);

            if (track?.ArtworkUri != null)
                builder.SetLargeIcon((await ImageService.Instance.LoadUrl(track.ArtworkUri).AsBitmapDrawableAsync()).Bitmap);

            return builder.Build();
        }

        private PendingIntent MediaButtonIntent(long action)
        {
            return MediaButtonReceiver.BuildMediaButtonPendingIntent(_context, action);
        }

        public const string NowPlayingChannel = "org.brunstad.bmm.NOW_PLAYING";
        public const int NowPlayingNotification = 0xb339;
    }
}