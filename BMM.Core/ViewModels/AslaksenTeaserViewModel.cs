using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.ViewModels
{
    public class AslaksenTeaserViewModel : PodcastBaseViewModel, IDarkStyleOnIosViewModel, IDarkStyleOnAndroidViewModel
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IAnalytics _analytics;
        private readonly Random _random = new Random();

        public const int FraBegynnelsenPodcastId = 49;
        public const int HebrewPodcastId = 25;
        public const string HebrewTagName = "hebreerbrevet";

        public const int AslaksenPodcastId = 30;
        public const string AsklaksenTagName = "aslaksen - troens ord";

        public IMvxCommand ShowAllCommand { get; }

        public IMvxAsyncCommand PlayRandomCommand { get; }

        public IMvxAsyncCommand PlayNewestCommand { get; }

        public override CacheKeys? CacheKey => CacheKeys.PodcastGetTracks;

        private bool _showTeaser;
        public bool ShowTeaser { get => _showTeaser; set => SetProperty(ref _showTeaser, value); }

        public AslaksenTeaserViewModel
            (
            IMvxNavigationService navigationService,
            IMediaPlayer mediaPlayer,
            IAnalytics analytics
            )
        {
            _mediaPlayer = mediaPlayer;
            _analytics = analytics;
            Podcast.Id = AslaksenPodcastId;

            ShowAllCommand = new ExceptionHandlingCommand(async () => {
                var podcast = Podcast;
                await navigationService.Navigate<PodcastViewModel, Podcast>(podcast);
            });

            PlayRandomCommand = new ExceptionHandlingCommand(async () =>
            {
                if (!Documents.Any())
                    return;

                var docs = Documents.OfType<IMediaTrack>().ToList();
                var randomIndex = _random.Next(docs.Count);
                var randomTrack = docs[randomIndex];

                await _mediaPlayer.Play(new List<IMediaTrack> {randomTrack}, randomTrack, PlaybackOriginString);

                _analytics.LogEvent("Aslaksen play random command was used");

                if(!_mediaPlayer.IsShuffleEnabled)
                    _mediaPlayer.ToggleShuffle();
            });

            PlayNewestCommand = new ExceptionHandlingCommand(async () =>
            {
                if (!Documents.Any())
                    return;

                var docs = Documents.OfType<IMediaTrack>().ToList();
                var filteredDocs = docs.Where(t => !t.IsListened).ToList();
                if (filteredDocs.Any())
                    docs = filteredDocs;

                await _mediaPlayer.Play(docs, docs.First(), PlaybackOriginString);

                _analytics.LogEvent("Aslaksen play newest command was used");
            });
        }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            var tracks = await Client.Podcast.GetTracks(Podcast.Id, policy);
            return tracks;
        }
    }
}