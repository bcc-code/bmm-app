using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
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
            
            _preparePlaybackHistoryAction.AttachDataContext(this);
        }

        public bool HasAnyEntry => Documents.Any();

        public override void Start()
        {
            base.Start();
            _analytics.LogEvent(string.Format(Event.ViewModelOpenedFormat, GetType().Name), new Dictionary<string, object>());
        }

        protected override async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            if (item is not TrackPO trackPO)
                return;

            var mediaTrack = (IMediaTrack)trackPO.Track;
            var index = list.IndexOf(trackPO.Track);
            await _mediaPlayer.Play(mediaTrack.EncloseInArray(), mediaTrack, PlaybackOriginString(index), mediaTrack.LastPosition);
        }

        public override async Task Load()
        {
            await base.Load();
            await RaisePropertyChanged(nameof(HasAnyEntry));
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return await _preparePlaybackHistoryAction.ExecuteGuarded();
        }
    }
}