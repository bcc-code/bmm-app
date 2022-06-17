using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.UI.iOS.Actions.Interfaces;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Utils;

namespace BMM.UI.iOS.Actions
{
    public class SearchForMusicFromSiriAction : 
        GuardedActionWithParameterAndResult<string, bool>,
        ISearchForMusicFromSiriAction
    {
        private const string SiriSearchPhraseAttributeName = "search_phrase";
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IPlayFromKareAction _playFromKareAction;
        private readonly ISearchClient _searchClient;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IPlaylistClient _playlistClient;
        private readonly IAlbumClient _albumClient;
        private readonly IContributorClient _contributorClient;
        private readonly IPodcastClient _podcastClient;
        private readonly IAnalytics _analytics;

        private string[] FromKareCurrentLanguageTranslation =>
            new[]
            {
                _bmmLanguageBinder[Translations.ExploreNewestViewModel_FraKaareHeader],
                _bmmLanguageBinder[Translations.Global_DailyMessage]
            };

        public SearchForMusicFromSiriAction(
            IBMMLanguageBinder bmmLanguageBinder, 
            IPlayFromKareAction playFromKareAction,
            ISearchClient searchClient,
            IMediaPlayer mediaPlayer,
            IPlaylistClient playlistClient,
            IAlbumClient albumClient,
            IContributorClient contributorClient,
            IPodcastClient podcastClient,
            IAnalytics analytics)
        {
            _bmmLanguageBinder = bmmLanguageBinder;
            _playFromKareAction = playFromKareAction;
            _searchClient = searchClient;
            _mediaPlayer = mediaPlayer;
            _playlistClient = playlistClient;
            _albumClient = albumClient;
            _contributorClient = contributorClient;
            _podcastClient = podcastClient;
            _analytics = analytics;
        }
        
        protected override async Task<bool> Execute(string searchPhrase)
        {
            _analytics.LogEvent(Event.SiriSearch, new Dictionary<string, object>()
            {
                {SiriSearchPhraseAttributeName, searchPhrase}
            });
            
            if (SiriConstants.FromKareSiriPhrases.Contains(searchPhrase) || FromKareCurrentLanguageTranslation.Contains(searchPhrase))
                return await _playFromKareAction.ExecuteGuarded();

            var docs = await _searchClient.GetAll(searchPhrase, 0, 1);
            
            if (docs == null || !docs.Items.Any())
                return false;

            var item = docs.Items.First();

            switch (item.DocumentType)
            {
                case DocumentType.Track:
                {
                    await _mediaPlayer.Play(((IMediaTrack)item).EncloseInArray(),
                        (IMediaTrack)item,
                        SiriUtils.CreatePlaybackOrigin(SiriSource.Search, searchPhrase));
                    
                    return true;
                }
                case DocumentType.Playlist:
                    return await PlayPlaylist((Playlist)item, searchPhrase);
                case DocumentType.Album:
                    return await PlayAlbum(item.Id, searchPhrase);
                case DocumentType.Contributor:
                    return await PlayContributor(searchPhrase, item);
                case DocumentType.Podcast:
                    return await PlayPodcast(searchPhrase, item);
            }

            return false;
        }

        private async Task<bool> PlayPodcast(string searchPhrase, Document item)
        {
            var tracks = await _podcastClient.GetTracks(item.Id, CachePolicy.IgnoreCache);
            return await PlayTracks(tracks, searchPhrase);
        }

        private async Task<bool> PlayContributor(string searchPhrase, Document item)
        {
            var tracks = await _contributorClient.GetTracks(item.Id, CachePolicy.IgnoreCache);
            return await PlayTracks(tracks, searchPhrase);
        }

        private async Task<bool> PlayPlaylist(Playlist item, string searchPhrase)
        {
           var tracks = await _playlistClient.GetTracks(item.Id, CachePolicy.IgnoreCache);
           
           if (tracks == null || !tracks.Any())
               return false;

           return await PlayTracks(tracks, searchPhrase);
        }

        private async Task<bool> PlayAlbum(int albumId, string searchPhrase)
        {
            var album = await _albumClient.GetById(albumId);

            if (album == null || !album.Children.Any())
                return false;

            var firstItem = album.Children.First();
            
            switch (firstItem.DocumentType)
            {
                case DocumentType.Track:
                    return await PlayTracks(album.Children.OfType<Track>(), searchPhrase);
                case DocumentType.Album:
                    return await PlayAlbum(firstItem.Id, searchPhrase);
            }

            return false;
        }
        
        private async Task<bool> PlayTracks(IEnumerable<Track> tracks, string searchPhrase)
        {
            var mediaTracks = tracks
                .OfType<IMediaTrack>()
                .ToList();

            await _mediaPlayer.Play(mediaTracks, mediaTracks.First(), SiriUtils.CreatePlaybackOrigin(SiriSource.Search, searchPhrase));
            return true;
        }
    }
}