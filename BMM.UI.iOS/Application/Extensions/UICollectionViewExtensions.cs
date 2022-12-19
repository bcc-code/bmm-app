using System.Linq;
using Foundation;
using UIKit;

namespace BMM.UI.iOS.Extensions
{
    public static class UICollectionViewExtensions
    {
        public static bool IsCellVisible(this UICollectionView collectionView, NSIndexPath indexPath)
        {
            return collectionView
                .VisibleCells
                .Any(c => collectionView.IndexPathForCell(c)!.Row == indexPath.Row);
        }
    }
}