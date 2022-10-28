using System;
using BMM.UI.iOS.Helpers;
using CoreGraphics;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS.CollectionViewSource
{
    public class YearInReviewSource : MvxCollectionViewSource, IUICollectionViewDelegateFlowLayout
    {
        private readonly int _collectionViewMargin;
        private readonly ICollectionViewSnapHandler _collectionViewSnapHandler;
        private readonly int _itemSpacing;
        private int _currentPageIndex;

        public YearInReviewSource(UICollectionView collectionView,
            NSString defaultCellIdentifier,
            int itemSpacing,
            int collectionViewMargin,
            ICollectionViewSnapHandler collectionViewSnapHandler) : base(collectionView, defaultCellIdentifier)
        {
            _itemSpacing = itemSpacing;
            _collectionViewMargin = collectionViewMargin;
            _collectionViewSnapHandler = collectionViewSnapHandler;
        }

        public nfloat ItemWidth { get; set; }
        public nfloat ItemHeight { get; set; }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CGSize(ItemWidth, ItemHeight);
        }

        [Export("collectionView:layout:insetForSectionAtIndex:")]
        public UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return new UIEdgeInsets(0,
                _collectionViewMargin,
                0,
                _collectionViewMargin);
        }

        [Export("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
        public nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return _itemSpacing;
        }

        [Export("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
        public void WillEndDragging(UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
        {
            nfloat pageWidth = ItemWidth + _itemSpacing;
            int newPageIndex;

            if (velocity.X > 0)
            {
                nfloat contentWidthWithoutCollectionMargins = scrollView.ContentSize.Width - 2 * _collectionViewMargin;
                var maxPageIndex = (int)Math.Ceiling(contentWidthWithoutCollectionMargins / pageWidth) - 1;
                newPageIndex = Math.Min(_currentPageIndex + 1, maxPageIndex);
            }
            else if (velocity.X == 0)
            {
                newPageIndex = (int)Math.Floor((targetContentOffset.X - pageWidth / 2) / pageWidth) + 1;
            }
            else
            {
                var minPageIndex = 0;
                newPageIndex = Math.Max(_currentPageIndex - 1, minPageIndex);
            }

            _currentPageIndex = newPageIndex;
            _collectionViewSnapHandler.OnPositionChanged(_currentPageIndex);
            targetContentOffset = new CGPoint(newPageIndex * pageWidth, targetContentOffset.Y);
        }
    }
}