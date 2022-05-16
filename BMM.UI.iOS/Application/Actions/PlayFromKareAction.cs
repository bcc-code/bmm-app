using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Analytics;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Actions.Interfaces;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Utils;

namespace BMM.UI.iOS.Actions
{
    public class PlayFromKareAction
        : GuardedActionWithResult<bool>,
          IPlayFromKareAction
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IPodcastClient _podcastClient;
        private readonly IAnalytics _analytics;

        public PlayFromKareAction(
            IMediaPlayer mediaPlayer,
            IPodcastClient podcastClient,
            IAnalytics analytics)
        {
            _mediaPlayer = mediaPlayer;
            _podcastClient = podcastClient;
            _analytics = analytics;
        }

        protected override async Task<bool> Execute()
        {
            _analytics.LogEvent(Event.FromKaarePlayed);
            
            var fromKareList = await _podcastClient.GetTracks(FraKaareTeaserViewModel.FraKårePodcastId, CachePolicy.IgnoreCache);
            
            if (fromKareList == null || !fromKareList.Any())
                return false;

            var fromKareMediaTracks = fromKareList
                .OfType<IMediaTrack>()
                .ToList();

            await _mediaPlayer.Play(fromKareMediaTracks, fromKareMediaTracks.First(), SiriUtils.CreatePlaybackOrigin(SiriSource.FraKaare));
            return true;
        }
    }
}