using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS.CollectionViewSource
{
    public class ProjectBoxAchievementsCollectionViewSource : MvxCollectionViewSource
    {
        public ProjectBoxAchievementsCollectionViewSource(UICollectionView collectionView) : base(collectionView)
        {
            collectionView.RegisterNibForCell(ProjectBoxAchievementCollectionViewCell.Nib, ProjectBoxAchievementCollectionViewCell.Key);
        }
        
        protected override UICollectionViewCell GetOrCreateCellFor(UICollectionView collectionView, NSIndexPath indexPath, object item)
        {
            return (UICollectionViewCell)collectionView.DequeueReusableCell(ProjectBoxAchievementCollectionViewCell.Key, indexPath);
        }
    }
}