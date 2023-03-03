using BMM.Core.Models.POs.Tiles;
using CoreImage;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS.CollectionViewSource
{
    public class TilesCollectionViewSource : MvxCollectionViewSource
    {
        public TilesCollectionViewSource(UICollectionView collectionView) : base(collectionView)
        {
            collectionView.RegisterNibForCell(ContinueListeningTileViewCell.Nib, ContinueListeningTileViewCell.Key);
            collectionView.RegisterNibForCell(MessageTileViewCell.Nib, MessageTileViewCell.Key);
            collectionView.RegisterNibForCell(VideoTileViewCell.Nib, VideoTileViewCell.Key);
        }

        public TilesCollectionViewSource(UICollectionView collectionView, NSString defaultCellIdentifier) : base(collectionView, defaultCellIdentifier)
        {
        }

        protected override UICollectionViewCell GetOrCreateCellFor(UICollectionView collectionView, NSIndexPath indexPath, object item)
        {
            string key = item switch
            {
                ContinueListeningTilePO _ => ContinueListeningTileViewCell.Key,
                MessageTilePO _ => MessageTileViewCell.Key,
                VideoTilePO _ => VideoTileViewCell.Key,
                _ => null
            };
            
            return (UICollectionViewCell)collectionView.DequeueReusableCell(key, indexPath);
        }
    }
}