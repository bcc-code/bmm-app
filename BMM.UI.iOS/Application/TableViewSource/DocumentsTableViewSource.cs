using Foundation;
using UIKit;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Carousels;
using BMM.Core.Models.POs.ContinueListening;
using BMM.Core.Models.POs.Contributors;
using BMM.Core.Models.POs.InfoMessages;
using BMM.Core.Models.POs.ListeningStreakPO;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Models.POs.Tracks;

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
                ContinueListeningCollectionTableViewCell.Key
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
                
                case ContinueListeningCollectionPO:
                    nibName = ContinueListeningCollectionTableViewCell.Key;
                    break;
            }

            return tableView.DequeueReusableCell(nibName);
        }
    }
}