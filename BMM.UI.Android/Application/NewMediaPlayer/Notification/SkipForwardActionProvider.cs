using _Microsoft.Android.Resource.Designer;
using Android.Support.V4.Media.Session;
using BMM.Core.NewMediaPlayer.Abstractions;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using MvvmCross;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification;

public class SkipForwardActionProvider : Java.Lang.Object, MediaSessionConnector.ICustomActionProvider
{
    public PlaybackStateCompat.CustomAction GetCustomAction(IPlayer player)
    {
        return new PlaybackStateCompat.CustomAction.Builder(
            NowPlayingNotificationBuilder.ActionSkipForward,
            NowPlayingNotificationBuilder.JumpForwardTitle,
            ResourceConstant.Drawable.icon_skip_forward_notification
        ).Build();
    }

    public void OnCustomAction(IPlayer player, string action, Bundle extras)
    {
        Mvx.IoCProvider.Resolve<IMediaPlayer>().JumpForward();
    }
}