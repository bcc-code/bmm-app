using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Factories.ContinueListening;
using BMM.Core.Implementations.Factories.DiscoverSection;
using BMM.Core.Implementations.Factories.HighlightedTextTracks;
using BMM.Core.Implementations.Factories.Streak;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.Factories.YearInReview;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Carousels;
using BMM.Core.Models.POs.Contributors;
using BMM.Core.Models.POs.InfoMessages;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.Models.POs.Recommendations;
using BMM.Core.Models.POs.Tiles;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Implementations.Factories
{
    public class DocumentsPOFactory : IDocumentsPOFactory
    {
        private readonly ITrackPOFactory _trackPOFactory;
        private readonly ITrackCollectionPOFactory _trackCollectionPOFactory;
        private readonly IListeningStreakPOFactory _listeningStreakPOFactory;
        private readonly IDiscoverSectionHeaderPOFactory _discoverSectionHeaderPOFactory;
        private readonly ITilePOFactory _tilePOFactory;
        private readonly IYearInReviewTeaserPOFactory _yearInReviewTeaserPOFactory;
        private readonly IHighlightedTextTrackPOFactory _highlightedTextTrackPOFactory;
        private readonly IPlaylistPOFactory _playlistPOFactory;
        private readonly IMvxNavigationService _mvxNavigationService;

        public DocumentsPOFactory(
            ITrackPOFactory trackPOFactory,
            ITrackCollectionPOFactory trackCollectionPOFactory,
            IListeningStreakPOFactory listeningStreakPOFactory,
            IDiscoverSectionHeaderPOFactory discoverSectionHeaderPOFactory,
            ITilePOFactory tilePOFactory,
            IYearInReviewTeaserPOFactory yearInReviewTeaserPOFactory,
            IHighlightedTextTrackPOFactory highlightedTextTrackPOFactory,
            IMvxNavigationService mvxNavigationService,
            IPlaylistPOFactory playlistPOFactory)
        {
            _trackPOFactory = trackPOFactory;
            _trackCollectionPOFactory = trackCollectionPOFactory;
            _listeningStreakPOFactory = listeningStreakPOFactory;
            _discoverSectionHeaderPOFactory = discoverSectionHeaderPOFactory;
            _tilePOFactory = tilePOFactory;
            _yearInReviewTeaserPOFactory = yearInReviewTeaserPOFactory;
            _highlightedTextTrackPOFactory = highlightedTextTrackPOFactory;
            _mvxNavigationService = mvxNavigationService;
            _playlistPOFactory = playlistPOFactory;
        }
        
        public IEnumerable<IDocumentPO> Create(
            IEnumerable<Document> documents,
            IMvxCommand<IDocumentPO> documentSelectedCommand,
            IMvxAsyncCommand<Document> optionsClickedCommand,
            ITrackInfoProvider trackInfoProvider)
        {
            var documentsPOList = new List<IDocumentPO>();

            foreach (var document in documents)
            {
                switch (document)
                {
                    case Track track:
                        documentsPOList.Add(_trackPOFactory.Create(trackInfoProvider, optionsClickedCommand, track));
                        break;
                    case HighlightedTextTrack highlightedTextTrack:
                        documentsPOList.Add(_highlightedTextTrackPOFactory.Create(highlightedTextTrack, optionsClickedCommand, trackInfoProvider));
                        break;
                    case Album album:
                        documentsPOList.Add(new AlbumPO(album));
                        break;
                    case Contributor contributor:
                        documentsPOList.Add(new ContributorPO(optionsClickedCommand, contributor));
                        break;
                    case TrackCollection trackCollection:
                        documentsPOList.Add(_trackCollectionPOFactory.Create(trackCollection));
                        break;
                    case Podcast podcast:
                        documentsPOList.Add(new PodcastPO(podcast));
                        break;
                    case PinnedItem pinnedItem:
                        documentsPOList.Add(new PinnedItemPO(pinnedItem));
                        break;
                    case ChapterHeader chapterHeader:
                        documentsPOList.Add(new ChapterHeaderPO(chapterHeader));
                        break;
                    case ContinueListeningTile continueListeningTile:
                        documentsPOList.Add(_tilePOFactory.Create(optionsClickedCommand, continueListeningTile));
                        break;
                    case CoverCarouselCollection coverCarouselCollection:
                        documentsPOList.Add(new CoverCarouselCollectionPO(this, trackInfoProvider, documentSelectedCommand, optionsClickedCommand, coverCarouselCollection));
                        break;
                    case TilesCollection continueListeningCollection:
                        documentsPOList.Add(new TileCollectionPO(_tilePOFactory, documentSelectedCommand, optionsClickedCommand, continueListeningCollection));
                        break;
                    case DiscoverSectionHeader discoverSectionHeader:
                        documentsPOList.Add(_discoverSectionHeaderPOFactory.Create(discoverSectionHeader));
                        break;
                    case Playlist playlist:
                        documentsPOList.Add(_playlistPOFactory.Create(playlist));
                        break;
                    case ListeningStreak listeningStreak:
                        documentsPOList.Add(_listeningStreakPOFactory.Create(listeningStreak));
                        break;
                    case InfoMessage infoMessage:
                        documentsPOList.Add(new InfoMessagePO(infoMessage));
                        break;
                    case YearInReviewTeaser yearInReviewTeaser:
                        documentsPOList.Add(_yearInReviewTeaserPOFactory.Create(yearInReviewTeaser));
                        break;
                    case Recommendation recommendation:
                        documentsPOList.Add(new RecommendationPO(recommendation, _trackPOFactory, trackInfoProvider, optionsClickedCommand, documentSelectedCommand));
                        break;
                    case ProjectBox projectBox:
                        documentsPOList.Add(new ProjectBoxPO(projectBox, _mvxNavigationService));
                        break;
                }
            }

            return documentsPOList;
        }
    }
}