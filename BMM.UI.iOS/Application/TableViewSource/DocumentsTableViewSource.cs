using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Carousels;
using BMM.Core.Models.POs.Contributors;
using BMM.Core.Models.POs.InfoMessages;
using BMM.Core.Models.POs.ListeningStreaks;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.Models.POs.Recommendations;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.YearInReview;

namespace BMM.UI.iOS
{
    public class DocumentsTableViewSource : LoadMoreTableViewSource
    {
        public DocumentsTableViewSource(UITableView tableView)
            : base(tableView)
        {
            string[] nibNames =
            {
                ContributorTableViewCell.Key,
                TrackTableViewCell.Key,
                TrackCollectionTableViewCell.Key,
                PinnedItemTableViewCell.Key,
                FeaturedPlaylistTableViewCell.Key,
                ChapterHeaderTableViewCell.Key,
                DiscoverSectionHeaderTableViewCell.Key,
                StreakTableViewCell.Key,
                PlaylistsCollectionTableViewCell.Key,
                InfoMessageTableViewCell.Key,
                SimpleMarginTableViewCell.Key,
                ContinueListeningCollectionTableViewCell.Key,
                YearInReviewTeaserCollapsedCell.Key,
                YearInReviewTeaserExpandedCell.Key,
                HighlightedTextTrackTableViewCell.Key,
                RecommendationTrackTableViewCell.Key,
                RecommendationContributorTableViewCell.Key,
                RecommendationAlbumTableViewCell.Key
            };
            
            foreach (string nibName in nibNames)
            {
                tableView.RegisterNibForCellReuse(UINib.FromName(nibName, NSBundle.MainBundle), nibName);
            }
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            string nibName = null;

            switch (item)
            {
                case ContributorPO:
                    nibName = ContributorTableViewCell.Key;
                    break;

                case HighlightedTextTrackPO:
                    nibName = HighlightedTextTrackTableViewCell.Key;
                    break;
                
                case TrackPO:
                    nibName = TrackTableViewCell.Key;
                    break;

                case TrackCollectionPO:
                    nibName = TrackCollectionTableViewCell.Key;
                    break;

                case PinnedItemPO:
                    nibName = PinnedItemTableViewCell.Key;
                    break;

                case ChapterHeaderPO:
                    nibName = ChapterHeaderTableViewCell.Key;
                    break;

                case DiscoverSectionHeaderPO:
                    nibName = DiscoverSectionHeaderTableViewCell.Key;
                    break;

                case AlbumPO:
                case PlaylistPO:
                case PodcastPO:
                    nibName = FeaturedPlaylistTableViewCell.Key;
                    break;

                case ListeningStreakPO:
                    nibName = StreakTableViewCell.Key;
                    break;

                case CoverCarouselCollectionPO:
                    nibName = PlaylistsCollectionTableViewCell.Key;
                    break;
                
                case InfoMessagePO:
                    nibName = InfoMessageTableViewCell.Key;
                    break;
                
                case SimpleMarginPO:
                    nibName = SimpleMarginTableViewCell.Key;
                    break;
                
                case TileCollectionPO:
                    nibName = ContinueListeningCollectionTableViewCell.Key;
                    break;

                case YearInReviewTeaserPO yearInReviewPreviewPO:
                {
                    nibName = yearInReviewPreviewPO.IsExpanded
                        ? YearInReviewTeaserExpandedCell.Key
                        : YearInReviewTeaserCollapsedCell.Key;
                    break;
                }
                
                case RecommendationPO recommendationPO:
                {
                    if (recommendationPO.TrackPO != null)
                        nibName = RecommendationTrackTableViewCell.Key;
                    else if (recommendationPO.ContributorPO != null)
                        nibName = RecommendationContributorTableViewCell.Key;
                    else if (recommendationPO.TrackListHolder != null)
                        nibName = RecommendationAlbumTableViewCell.Key;
                    
                    break;
                }
            }

            return tableView.DequeueReusableCell(nibName);
        }
    }
}