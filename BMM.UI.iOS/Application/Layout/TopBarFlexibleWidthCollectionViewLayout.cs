using System.Linq;
using BMM.UI.iOS.Extensions;
using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Layout
{
    public sealed class TopBarFlexibleWidthCollectionViewLayout : UICollectionViewFlowLayout
    {
        private readonly CGSize _estimatedSizeAutomatic = new CGSize(50, 56);

        public TopBarFlexibleWidthCollectionViewLayout()
        {
            SectionInset = UIEdgeInsets.Zero;
            MinimumInteritemSpacing = 0;
            MinimumLineSpacing = 0;
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            EstimatedItemSize = _estimatedSizeAutomatic;
        }

        public override CGSize CollectionViewContentSize => base.CollectionViewContentSize.AddWidth(16);
    }
}