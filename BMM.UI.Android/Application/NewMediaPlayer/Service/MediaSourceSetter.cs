using System;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Service
{
    public class MediaSourceSetter
    {
        private readonly Func<MediaSessionConnector> _mediaSessionConnector;
        private readonly Func<ConcatenatingMediaSource, TimelineQueueEditor> _queueEditorFactory;
        private ConcatenatingMediaSource _mediaSource;

        public MediaSourceSetter(Func<MediaSessionConnector> mediaSessionConnector, Func<ConcatenatingMediaSource, TimelineQueueEditor> queueEditorFactory)
        {
            _mediaSessionConnector = mediaSessionConnector;
            _queueEditorFactory = queueEditorFactory;
        }

        public ConcatenatingMediaSource Get => _mediaSource;

        public void CreateNew(int startIndex, params IMediaSource[] mediaSources)
        {
            CreateNew(new CustomShuffleOrder(startIndex, mediaSources.Length), mediaSources);
        }

        public void CreateNew(params IMediaSource[] mediaSources)
        {
            CreateNew(new CustomShuffleOrder(), mediaSources);
        }

        private void CreateNew(CustomShuffleOrder shuffleOrder, params IMediaSource[] mediaSources)
        {
            _mediaSource = new ConcatenatingMediaSource(false, shuffleOrder, mediaSources);
            var queueEditor = _queueEditorFactory.Invoke(_mediaSource);
            _mediaSessionConnector().SetQueueEditor(queueEditor);
        }
    }
}