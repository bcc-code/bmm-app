using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Other;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS.CollectionViewSource
{
    public class ProfileAchievementsCollectionViewSource : MvxCollectionViewSource
    {
        private static CGSize AchievementCellSize = new(72, 72);
        private const int HeaderCellHeight = 48;
        
        public ProfileAchievementsCollectionViewSource(UICollectionView collectionView) : base(collectionView)
        {
            collectionView.RegisterNibForCell(ProfileAchievementsCollectionViewCell.Nib, ProfileAchievementsCollectionViewCell.Key);
            collectionView.RegisterNibForCell(HeaderCollectionViewCell.Nib, HeaderCollectionViewCell.Key);
        }

        public ProfileAchievementsCollectionViewSource(UICollectionView collectionView, NSString defaultCellIdentifier) : base(collectionView, defaultCellIdentifier)
        {
        }
        
        protected override UICollectionViewCell GetOrCreateCellFor(UICollectionView collectionView, NSIndexPath indexPath, object item)
        {
            string key = item switch
            {
                AchievementPO _ => ProfileAchievementsCollectionViewCell.Key,
                ChapterHeaderPO _ => HeaderCollectionViewCell.Key,
                _ => null
            };
            
            return (UICollectionViewCell)collectionView.DequeueReusableCell(key, indexPath);
        }
        
        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            object item = GetItemAt(indexPath);
            if (item is AchievementPO)
                return AchievementCellSize;

            return new CGSize(collectionView.Frame.Width, HeaderCellHeight);
        }
    }
}