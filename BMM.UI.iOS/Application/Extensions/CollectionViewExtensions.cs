using System;
using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Extensions
{
    public static class CollectionViewExtensions
    {
        public static void SetXOffset(this UICollectionView collectionView, nfloat newX, bool animated)
        {
            var point = new CGPoint(
                newX,
                collectionView.ContentOffset.Y);

            collectionView.SetContentOffset(point, animated);
        }
    }
}