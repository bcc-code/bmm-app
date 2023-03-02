using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;

namespace BMM.UI.Droid.Application.NewMediaPlayer
{
    /// <summary>
    /// Sets the MediaId from metadata
    /// </summary>
    public class MetadataReadingQueueNavigator : TimelineQueueNavigator
    {
        private readonly IMetadataMapper _metadataMapper;
        private readonly Timeline.Window _window;

        public MetadataReadingQueueNavigator(MediaSessionCompat mediaSession, IMetadataMapper metadataMapper) : base(mediaSession)
        {
            _metadataMapper = metadataMapper;
            _window = new Timeline.Window();
        }

        public override MediaDescriptionCompat GetMediaDescription(IPlayer player, int windowIndex)
        {
            var item = player!.CurrentTimeline!.GetWindow(windowIndex, _window);
            var extras = item!.MediaItem!.MediaMetadata!.Extras!;
            return _metadataMapper.FromBundle(extras);
        }
    }
}