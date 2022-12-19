using System;
using BMM.UI.iOS.CollectionTableViewCell;
using BMM.UI.iOS.Delegates;
using CoreGraphics;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS.CollectionViewSource
{
    public class ControllersCollectionViewSource : MvxCollectionViewSource
    {
        private readonly TopBarCollectionViewSource _topBarCollectionView;
        private readonly CreateOrRefreshViewControllerDelegate _createOrRefreshViewController;

        private nfloat? _lastScrollPosition;
        private int? _previouslyAnimatedIndex;

        public ControllersCollectionViewSource(
            UICollectionView collectionView,
            TopBarCollectionViewSource topBarCollectionViewSource,
            NSString defaultCellIdentifier,
            CreateOrRefreshViewControllerDelegate createOrRefreshViewController)
            : base(collectionView, defaultCellIdentifier)
        {
            _topBarCollectionView = topBarCollectionViewSource;
            _createOrRefreshViewController = createOrRefreshViewController;
            CollectionView.PagingEnabled = true;
            CollectionView.ScrollEnabled = true;
            collectionView.RegisterNibForCell(ControllerCollectionViewCell.Nib, ControllerCollectionViewCell.Key);
        }

        public event EventHandler ScrollAnimationEndedEvent;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            var cell = (ControllerCollectionViewCell) GetOrCreateCellFor(collectionView, indexPath, item);

            cell.InitializeCell(
                this,
                _createOrRefreshViewController,
                item);

            return cell;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            if (!CollectionView.Dragging && !CollectionView.Decelerating)
                return;

            int currentIndex = GetCurrentIndexBasedOnScrollPosition();

            if (_lastScrollPosition == null)
            {
                _lastScrollPosition = CollectionView.ContentOffset.X;
                _previouslyAnimatedIndex = currentIndex;
                return;
            }

            bool indexChanged = _previouslyAnimatedIndex != currentIndex;
            _previouslyAnimatedIndex = indexChanged ? currentIndex : _previouslyAnimatedIndex;
            
            if (indexChanged)
                _topBarCollectionView.UpdateSelection(currentIndex);
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            _lastScrollPosition = null;
            _previouslyAnimatedIndex = null;
            SelectedItem = GetItemAt(GetCurrentPath());
        }

        private int GetCurrentIndexBasedOnScrollPosition()
        {
            return (int)((CollectionView.ContentOffset.X + CollectionView.Frame.Size.Width / 2 - 1)
                         / CollectionView.Frame.Size.Width);
        }

        private NSIndexPath GetCurrentPath()
        {
            int columnIndex = GetCurrentIndexBasedOnScrollPosition();
            return NSIndexPath.FromRowSection(columnIndex, 0);
        }

        public override void WillDisplayCell(
            UICollectionView collectionView,
            UICollectionViewCell cell,
            NSIndexPath indexPath)
        {
            if (cell is ControllerCollectionViewCell collectionCell)
                collectionCell.NotifyControllerWillAppear();
        }

        public override void CellDisplayingEnded(
            UICollectionView collectionView,
            UICollectionViewCell cell,
            NSIndexPath indexPath)
        {
            if (cell is ControllerCollectionViewCell collectionCell)
                collectionCell.NotifyControllerWillDisappear();
        }

        public override void ScrollAnimationEnded(UIScrollView scrollView)
        {
            ScrollAnimationEndedEvent?.Invoke(this, EventArgs.Empty);
        }

        #region IUICollectionViewDelegateFlowLayout

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(
            UICollectionView collectionView,
            UICollectionViewLayout layout,
            NSIndexPath indexPath)
        {
            return collectionView.Frame.Size;
        }

        #endregion
    }
}