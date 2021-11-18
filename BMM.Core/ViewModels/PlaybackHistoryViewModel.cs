using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.ViewModels
{
    public class PlaybackHistoryViewModel
        : DocumentsViewModel,
          IPlaybackHistoryViewModel
    {
        private readonly IPreparePlaybackHistoryAction _preparePlaybackHistoryAction;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IAnalytics _analytics;

        public PlaybackHistoryViewModel(
            IPreparePlaybackHistoryAction preparePlaybackHistoryAction,
            IMediaPlayer mediaPlayer,
            IAnalytics analytics)
        {
            _preparePlaybackHistoryAction = preparePlaybackHistoryAction;
            _mediaPlayer = mediaPlayer;
            _analytics = analytics;
        }

        public bool HasAnyEntry => Documents.Any();

        public override void Start()
        {
            base.Start();
            _analytics.LogEvent(string.Format(Event.ViewModelOpenedFormat, GetType().Name), new Dictionary<string, object>());
        }

        protected override async Task DocumentAction(Document item, IList<Track> list)
        {
            if (!(item is IMediaTrack mediaTrack))
                return;

            await _mediaPlayer.Play(mediaTrack.EncloseInArray(), mediaTrack, PlaybackOriginString, mediaTrack.LastPosition);
        }

        public override async Task Load()
        {
            await base.Load();
            await RaisePropertyChanged(nameof(HasAnyEntry));
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return await _preparePlaybackHistoryAction.ExecuteGuarded();
        }
    }
}