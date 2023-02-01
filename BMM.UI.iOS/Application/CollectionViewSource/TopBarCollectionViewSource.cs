using System;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Constants;
using BMM.UI.iOS.Extensions;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS.CollectionViewSource
{
    public class TopBarCollectionViewSource : MvxCollectionViewSource
    {
        private const double AnimationDuration = 0.3;
        private const int ChangeContentOffsetStepValue = 10;
        private readonly UICollectionView _controllersCollectionView;
        private readonly UIView _selectorView;
        private readonly UIView _parentView;
        private NSLayoutConstraint _selectorViewXConstraint;
        private NSLayoutConstraint _selectorWidthConstraint;
        private IDisposable _actionItemsSourceSubscription;

        public TopBarCollectionViewSource(
            UICollectionView collectionView,
            UICollectionView controllersCollectionView,
            UIView selectorView,
            UIView parentView,
            NSString defaultCellIdentifier) : base(collectionView, defaultCellIdentifier)
        {
            _controllersCollectionView = controllersCollectionView;
            _selectorView = selectorView;
            _parentView = parentView;
            _selectorView.Hidden = true;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemSelected(indexPath, true);
            base.ItemSelected(collectionView, indexPath);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => ItemsSource.Count();

        protected override UICollectionViewCell GetOrCreateCellFor(
            UICollectionView collectionView,
            NSIndexPath indexPath,
            object item)
        {
            var key = DefaultCellIdentifier;
            return (UICollectionViewCell)collectionView.DequeueReusableCell(key, indexPath);
        }

        private async void ItemSelected(NSIndexPath indexPath, bool animate)
        {
            var contentOffset = CollectionView.ContentOffset.X;

            _controllersCollectionView.ScrollToItem(
                indexPath,
                UICollectionViewScrollPosition.None,
                ShouldAnimateScrollForControllers(indexPath, animate));

            if (!CollectionView.IsCellVisible(indexPath))
            {
                if (indexPath.Row == NumericConstants.Zero)
                {
                    CollectionView.SetContentOffset(
                        new CGPoint(NumericConstants.Zero, CollectionView.ContentOffset.Y),
                        false);
                }
                else
                {
                    var indexPathsForVisibleItems = CollectionView.IndexPathsForVisibleItems;

                    if (indexPathsForVisibleItems.Length != NumericConstants.Zero)
                    {
                        int directionFactor = GetDirectionFactor(indexPath, indexPathsForVisibleItems);

                        do
                        {
                            var changeContentTcs = new TaskCompletionSource<bool>();

                            InvokeInBackground(() =>
                                {
                                    contentOffset = ScrollToNextOffsetStep(contentOffset, directionFactor, changeContentTcs);
                                });

                            await changeContentTcs.Task;
                        } while (!CollectionView.IsCellVisible(indexPath)
                                 && contentOffset < CollectionView.ContentSize.Width
                                 && contentOffset > NumericConstants.Zero);
                    }
                }
            }

            CollectionView.ScrollToItem(
                indexPath,
                UICollectionViewScrollPosition.CenteredHorizontally,
                animate);

            UpdateSelectedBar(indexPath, animate);
            SelectedIndexPath = indexPath;
        }

        private int GetDirectionFactor(NSIndexPath indexPath, NSIndexPath[] indexPathsForVisibleItems)
            => indexPath.Row < indexPathsForVisibleItems.Select(i => i.Row).Min()
                ? -1
                : 1;

        private nfloat ScrollToNextOffsetStep(nfloat contentOffset, int directionFactor, TaskCompletionSource<bool> changeContentTcs)
        {
            contentOffset += ChangeContentOffsetStepValue * directionFactor;

            InvokeOnMainThread(
                () =>
                {
                    CollectionView.SetContentOffset(
                        new CGPoint(contentOffset, CollectionView.ContentOffset.Y),
                        false);

                    changeContentTcs.SetResult(true);
                });
            return contentOffset;
        }

        private bool ShouldAnimateScrollForControllers(NSIndexPath indexPath, bool animate)
        {
            if (SelectedIndexPath is null)
                return animate;

            if (Math.Abs(SelectedIndexPath.Row - indexPath.Row) == 1)
                return animate;

            return false;
        }

        public override void ReloadData()
        {
            base.ReloadData();

            if (ItemsSource != null && ItemsSource.Count() > 0)
            {
                _selectorView.Hidden = false;
                UpdateSelectedBar(NSIndexPath.FromRowSection(0, 0), false);
            }
            else
            {
                _selectorView.Hidden = true;
            }
        }

        public NSIndexPath SelectedIndexPath { get; set; }

        public UICollectionViewLayoutAttributes GetLayoutAttributesForItem(NSIndexPath indexPath)
        {
            return CollectionView.GetLayoutAttributesForItem(indexPath);
        }

        public void UpdateSelectedBar(NSIndexPath indexPath, bool animate)
        {
            var cell = CollectionView.CellForItem(indexPath);

            if (cell is null)
                return;

            UpdateSelectorViewConstraints(cell, animate);
        }

        private void UpdateSelectorViewConstraints(
            UICollectionViewCell cell,
            bool animate)
        {
            try
            {
                if (animate)
                    UIView.Animate(AnimationDuration, UpdateConstraints);
                else
                    UpdateConstraints();

                _selectorView.Hidden = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //yup, this sometimes crashes causing problems with reloading views; if caught it's well rendered then
            }

            void UpdateConstraints()
            {
                var currentSelectorViewXConstraint = _selectorView.CenterXAnchor.ConstraintEqualTo(cell.CenterXAnchor);
                var currentSelectorViewWidthConstraint = _selectorView.WidthAnchor.ConstraintEqualTo(cell.WidthAnchor);

                SetConstraintsForAnchor(
                    ref _selectorViewXConstraint,
                    currentSelectorViewXConstraint);

                SetConstraintsForAnchor(
                    ref _selectorWidthConstraint,
                    currentSelectorViewWidthConstraint);

                _parentView.SetNeedsUpdateConstraints();
                _parentView.UpdateConstraints();

                if (animate)
                    _parentView.LayoutIfNeeded();
            }

            void SetConstraintsForAnchor(ref NSLayoutConstraint currentConstraint, NSLayoutConstraint newConstraint)
            {
                var previousSelectorViewConstraint = currentConstraint;
                currentConstraint = newConstraint;

                if (previousSelectorViewConstraint != null)
                    previousSelectorViewConstraint.Active = false;

                currentConstraint.Active = true;
            }
        }

        public override void CellDisplayingEnded(
            UICollectionView collectionView,
            UICollectionViewCell cell,
            NSIndexPath indexPath)
        {
            if (IsSelectedItem(indexPath))
                _selectorView.Hidden = true;
        }

        public override void WillDisplayCell(
            UICollectionView collectionView,
            UICollectionViewCell cell,
            NSIndexPath indexPath)
        {
            if (IsSelectedItem(indexPath))
                UpdateSelectorViewConstraints(cell, false);
        }

        private bool IsSelectedItem(NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            bool isSelected = item == SelectedItem;
            return isSelected;
        }

        public void UpdateSelection(int currentIndex)
        {
            var currentlyAnimatedIndex = NSIndexPath.FromItemSection(currentIndex, 0);
            ItemSelected(CollectionView, currentlyAnimatedIndex);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_actionItemsSourceSubscription != null)
                {
                    _actionItemsSourceSubscription.Dispose();
                    _actionItemsSourceSubscription = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}