using Com.Google.Android.Exoplayer2;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Playback
{
    public static class PlayerExtensions
    {
        /// <summary>
        /// Is it a livestream that does not support seeking.
        /// See https://github.com/google/ExoPlayer/issues/2668 for more information. But duration is not <see cref="C.TimeUnset"/> and we therefore don't use the proposed way.
        /// </summary>
        public static bool IsLiveStream(this IPlayer player)
        {
            return player.IsCurrentWindowDynamic;
        }
    }
}