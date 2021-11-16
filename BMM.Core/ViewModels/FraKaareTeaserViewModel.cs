using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.ViewModels
{
    public class FraKaareTeaserViewModel : PodcastBaseViewModel, IDarkStyleOnIosViewModel, IDarkStyleOnAndroidViewModel
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IAnalytics _analytics;
        private readonly ISettingsStorage _settings;
        public const int FraKårePodcastId = 1;
        public const string FromKaareTagName = "fra-kaare";

        public IMvxCommand ShowAllCommand { get; }

        public override CacheKeys? CacheKey => CacheKeys.PodcastGetTracks;

        private bool _showTeaser;
        private CellWrapperViewModel<Document> _track;

        public bool ShowTeaser { get => _showTeaser; set => SetProperty(ref _showTeaser, value); }

        public IMvxAsyncCommand PlayRandomCommand { get; }

        public CellWrapperViewModel<Document> Track
        {
            get => _track;
            private set => SetProperty(ref _track, value);
        }

        public IMvxAsyncCommand TrackClickedCommand { get; }

        public FraKaareTeaserViewModel(IMvxNavigationService navigationService, IMediaPlayer mediaPlayer, IAnalytics analytics, ISettingsStorage settings)
        {
            _mediaPlayer = mediaPlayer;
            _analytics = analytics;
            _settings = settings;
            Podcast.Id = FraKårePodcastId;
            ShowAllCommand = new ExceptionHandlingCommand(async () =>
            {
                var podcast = Podcast;
                await navigationService.Navigate<PodcastViewModel, Podcast>(podcast);
            });
            PlayRandomCommand = new ExceptionHandlingCommand(async () =>
            {
                var randomEpisode = await Client.Podcast.GetRandomTrack(Podcast.Id);

                await _mediaPlayer.Play(new List<IMediaTrack> {randomEpisode}, randomEpisode, PlaybackOriginString);

                _analytics.LogEvent("Fra Kaare play random command was used");
            });

            TrackClickedCommand = new ExceptionHandlingCommand(async () =>
            {
                DocumentSelectedCommand?.Execute(Track.Item);
            });
        }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            var items = await Client.Podcast.GetTracks(Podcast.Id, policy);

            var item = items.FirstOrDefault();

            if (item != null)
                Track = new CellWrapperViewModel<Document>(item, this);

            return items.Take(1).ToList();
        }

        protected override async Task DocumentAction(Document item, IList<Track> list)
        {
            await base.DocumentAction(item, list);

            if (await ShouldPlaySongsOfFirstPlaylist())
                ExceptionHandler.FireAndForgetWithoutUserMessages(EnqueueSongsOfFirstPlaylist);
        }

        private Task<bool> ShouldPlaySongsOfFirstPlaylist()
        {
            return _settings.GetAutoplayEnabled();
        }

        private async Task EnqueueSongsOfFirstPlaylist()
        {
            var tracksToBePlayed = await GetSongsOfFirstPlaylist();
            ShuffleableQueue.ShuffleList(tracksToBePlayed, new Random());

            foreach (var track in tracksToBePlayed)
            {
                await _mediaPlayer.AddToEndOfQueue(track, PlaybackOriginString);
            }
        }

        private async Task<IList<Track>> GetSongsOfFirstPlaylist()
        {
            var playlists = await Client.Playlist.GetAll(CachePolicy.UseCacheAndRefreshOutdated);

            if (!playlists.Any())
                return new List<Track>();

            var firstPlaylist = playlists.First();
            var tracks = await Client.Playlist.GetTracks(firstPlaylist.Id, CachePolicy.UseCache);

            return tracks;
        }
    }
}