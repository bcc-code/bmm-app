using Android.Support.V4.Media.Session;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.UI.Droid.Application.NewMediaPlayer
{
    public class PlaybackStateCompatMapper
    {
        public RepeatType ConvertRepeatMode(int? repeatMode)
        {
            switch (repeatMode)
            {
                case PlaybackStateCompat.RepeatModeOne:
                    return RepeatType.RepeatOne;
                case PlaybackStateCompat.RepeatModeAll:
                case PlaybackStateCompat.RepeatModeGroup:
                    return RepeatType.RepeatAll;
                default:
                    return RepeatType.None;
            }
        }

        public int ConvertRepeatType(RepeatType type)
        {
            switch (type)
            {
                case RepeatType.RepeatAll:
                    return PlaybackStateCompat.RepeatModeAll;
                case RepeatType.RepeatOne:
                    return PlaybackStateCompat.RepeatModeOne;
                default:
                    return PlaybackStateCompat.RepeatModeNone;
            }
        }
    }
}