using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using Foundation;
using System.Reflection;
using MvvmCross.ViewModels;
using UIKit;
using BMM.Core.Implementations.Analytics;
using System.Collections.Generic;
using MvvmCross;

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
                FraKaareTableViewCell.Key,
                InfoMessageTableViewCell.Key,
                AslaksenTableViewCell.Key,
                BMMRadioTableViewCell.Key,
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
            PropertyInfo documentProperty = item.GetType().GetProperty("Item");
            Document document = documentProperty?.GetValue(item) as Document;

            if (document == null)
            {
                IAnalytics analytics = Mvx.IoCProvider.Resolve<IAnalytics>();
                string eventString = "4932 GetOrCreateCellFor null reference";

                analytics.LogEvent(eventString,
                    new Dictionary<string, object>
                    {
                        {"info", "document is null"}
                    });
                return tableView.DequeueReusableCell(TrackTableViewCell.Key);
            }

            string nibName = null;

            switch (document.DocumentType)
            {
                case DocumentType.Contributor:
                    nibName = ContributorTableViewCell.Key;
                    break;

                case DocumentType.Track:
                    nibName = TrackTableViewCell.Key;
                    break;

                case DocumentType.TrackCollection:
                    nibName = TrackCollectionTableViewCell.Key;
                    break;

                case DocumentType.PinnedItem:
                    nibName = PinnedItemTableViewCell.Key;
                    break;

                case DocumentType.ChapterHeader:
                    nibName = ChapterHeaderTableViewCell.Key;
                    break;

                case DocumentType.DiscoverSectionHeader:
                    nibName = DiscoverSectionHeaderTableViewCell.Key;
                    break;

                case DocumentType.Album:
                case DocumentType.Playlist:
                case DocumentType.Podcast:
                    nibName = FeaturedPlaylistTableViewCell.Key;
                    break;

                case DocumentType.ListeningStreak:
                    nibName = StreakTableViewCell.Key;
                    break;

                case DocumentType.PlaylistsCollection:
                    nibName = PlaylistsCollectionTableViewCell.Key;
                    break;
                
                case DocumentType.FraKaareTeaser:
                    nibName = FraKaareTableViewCell.Key;
                    break;
                
                case DocumentType.AslaksenTeaser:
                    nibName = AslaksenTableViewCell.Key;
                    break;
                
                case DocumentType.LiveRadio:
                    nibName = BMMRadioTableViewCell.Key;
                    break;
                
                case DocumentType.InfoMessage:
                    nibName = InfoMessageTableViewCell.Key;
                    break;
                
                case DocumentType.SimpleMargin:
                    nibName = SimpleMarginTableViewCell.Key;
                    break;
                
                case DocumentType.ContinueListeningCollection:
                    nibName = ContinueListeningCollectionTableViewCell.Key;
                    break;
            }

            return tableView.DequeueReusableCell(nibName);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            MvxObservableCollection<CellWrapperViewModel<Document>> data = ItemsSource as MvxObservableCollection<CellWrapperViewModel<Document>>;

            if (data == null || indexPath.Row < data.Count)
            {
                base.RowSelected(tableView, indexPath);
            }
        }
    }
}