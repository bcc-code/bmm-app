using System;
using System.Linq;
using BMM.Core.Constants;
using BMM.Core.Interactions.Base;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.UI.iOS.CollectionTableViewCell;
using BMM.UI.iOS.CollectionViewSource;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Layout;
using CoreGraphics;
using Foundation;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public abstract class ViewPagerBaseController<TViewModel, TItem>
        : BaseViewController<TViewModel>
        where TViewModel : BaseViewModel, ICollectionViewModel<TItem>
    {
        private const float TopBarHeight = TopBarItemsHeight + GradientHeight;
        private const float TopBarItemsHeight = 56;
        private const float GradientHeight = 2;
        private const float SelectorWidth = 20;
        private const float SeparatorHeight = 1;

        private ControllersCollectionViewLayout _controllersCollectionViewLayout;
        private ControllersCollectionViewSource _controllersCollectionViewSource;
        private UIView _selectorView;
        private UIView _separatorView;
        private IDisposable _controllersBoundsChangeObserver;
        private IDisposable _controllersFrameChangeObserver;
        protected UICollectionView TopBarCollectionView;
        protected UICollectionView ControllersCollectionView;
        protected TopBarCollectionViewSource TopBarCollectionViewSource;
        private IBmmInteraction _resetInteraction;

        protected ViewPagerBaseController(string nibName) : base(nibName)
        {
        }

        protected abstract UIView HostViewForPager { get; }
        protected abstract UICollectionViewFlowLayout TopBarCollectionViewLayout { get; }
        protected abstract NSString TopBarCollectionViewCellKey { get; }
        
        public IBmmInteraction ResetInteraction
        {
            get => _resetInteraction;
            set
            {
                if (_resetInteraction != null)
                    _resetInteraction.Requested -= ResetInteractionOnRequested;

                _resetInteraction = value;
                _resetInteraction.Requested += ResetInteractionOnRequested;
            }
        }

        private void ResetInteractionOnRequested(object sender, EventArgs e)
        {
            ControllersCollectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.None, false);
            TopBarCollectionView.SelectItem(NSIndexPath.FromRowSection(0, 0), false, UICollectionViewScrollPosition.None);
            TopBarCollectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.None, false);
            TopBarCollectionViewSource.UpdateSelectedBar(NSIndexPath.FromRowSection(0, 0), false);
        }

        protected virtual void Bind(MvxFluentBindingDescriptionSet<BaseViewController<TViewModel>, TViewModel> set)
        {
            set.Bind(TopBarCollectionViewSource)
                .For(p => p.ItemsSource)
                .To(vm => vm.CollectionItems);
            
            set.Bind(_controllersCollectionViewSource)
                .For(p => p.ItemsSource)
                .To(vm => vm.CollectionItems);

            set.Bind(TopBarCollectionViewSource)
                .For(p => p.SelectedItem)
                .To(vm => vm.SelectedCollectionItem);
            
            set.Bind(_controllersCollectionViewSource)
                .For(p => p.SelectedItem)
                .To(vm => vm.SelectedCollectionItem);
            
            set.Bind(this)
                .For(p => p.ResetInteraction)
                .To(vm => vm.ResetInteraction);
        }

        private void AddConstraints()
        {
            HostViewForPager.TranslatesAutoresizingMaskIntoConstraints = false;
            TopBarCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            ControllersCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            _selectorView.TranslatesAutoresizingMaskIntoConstraints = false;
            _separatorView.TranslatesAutoresizingMaskIntoConstraints = false;

            NSLayoutConstraint.ActivateConstraints(
                new[]
                {
                    TopBarCollectionView.LeadingAnchor.ConstraintEqualTo(HostViewForPager.LeadingAnchor),
                    TopBarCollectionView.TrailingAnchor.ConstraintEqualTo(HostViewForPager.TrailingAnchor),
                    TopBarCollectionView.TopAnchor.ConstraintEqualTo(HostViewForPager.TopAnchor),
                    TopBarCollectionView.HeightAnchor.ConstraintEqualTo(TopBarHeight),
                    
                    _separatorView.LeadingAnchor.ConstraintEqualTo(TopBarCollectionView.LeadingAnchor),
                    _separatorView.TrailingAnchor.ConstraintEqualTo(TopBarCollectionView.TrailingAnchor),
                    _separatorView.TopAnchor.ConstraintEqualTo(TopBarCollectionView.BottomAnchor),
                    _separatorView.HeightAnchor.ConstraintEqualTo(SeparatorHeight),

                    ControllersCollectionView.LeadingAnchor.ConstraintEqualTo(HostViewForPager.LeadingAnchor),
                    ControllersCollectionView.TrailingAnchor.ConstraintEqualTo(HostViewForPager.TrailingAnchor),
                    ControllersCollectionView.TopAnchor.ConstraintEqualTo(
                        _separatorView.BottomAnchor,
                        NumericConstants.Zero),
                    ControllersCollectionView.BottomAnchor.ConstraintEqualTo(
                        HostViewForPager.BottomAnchor,
                        NumericConstants.Zero),

                    _selectorView.TopAnchor.ConstraintEqualTo(TopBarCollectionView.TopAnchor, TopBarItemsHeight),
                    _selectorView.WidthAnchor.ConstraintEqualTo(SelectorWidth),
                    _selectorView.HeightAnchor.ConstraintEqualTo(GradientHeight),
                });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TopBarCollectionView = new UICollectionView(CGRect.Empty, TopBarCollectionViewLayout)
            {
                BackgroundColor = UIColor.Blue
            };

            _controllersCollectionViewLayout =
                new ControllersCollectionViewLayout { ScrollDirection = UICollectionViewScrollDirection.Horizontal };

            ControllersCollectionView = new UICollectionView(
                new CGRect(),
                _controllersCollectionViewLayout);

            _selectorView = new UIView()
            {
                BackgroundColor = AppColors.LabelPrimaryColor
            };

            _separatorView = new UIView()
            {
                BackgroundColor = AppColors.SeparatorColor
            };
                
            TopBarCollectionViewSource = new TopBarCollectionViewSource(
                TopBarCollectionView,
                ControllersCollectionView,
                _selectorView,
                HostViewForPager,
                TopBarCollectionViewCellKey);

            TopBarCollectionView.Source = TopBarCollectionViewSource;
            TopBarCollectionView.BackgroundColor = UIColor.Clear;
            TopBarCollectionView.ShowsHorizontalScrollIndicator = false;
            TopBarCollectionView.Add(_selectorView);

            _controllersCollectionViewSource = new ControllersCollectionViewSource(
                ControllersCollectionView,
                TopBarCollectionViewSource,
                ControllerCollectionViewCell.Key,
                CreateOrRefreshViewController);
            ControllersCollectionView.Source = _controllersCollectionViewSource;
            ControllersCollectionView.BackgroundColor = UIColor.Clear;

            HostViewForPager.Add(TopBarCollectionView);
            HostViewForPager.Add(ControllersCollectionView);
            HostViewForPager.Add(_separatorView);
            
            AddConstraints();
            var set = this.CreateBindingSet<BaseViewController<TViewModel>, TViewModel>();

            Bind(set);
            set.Apply();
        }

        protected abstract MvxViewController CreateOrRefreshViewController(
            object item,
            MvxViewController existingController = null);

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var visibleCells = ControllersCollectionView.VisibleCells;
            if (visibleCells?.Any() ?? false)
                (visibleCells.First() as ControllerCollectionViewCell)?.NotifyControllerWillAppear();

            _controllersBoundsChangeObserver = ControllersCollectionView.AddObserver(
                "bounds",
                NSKeyValueObservingOptions.New,
                ControllersCollectionViewBoundsObserver);

            _controllersFrameChangeObserver = ControllersCollectionView.AddObserver(
                "frame",
                NSKeyValueObservingOptions.New,
                ControllersCollectionViewBoundsObserver);
        }
        
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var visibleCells = ControllersCollectionView.VisibleCells;
            if (visibleCells?.Any() ?? false)
                (visibleCells.First() as ControllerCollectionViewCell)?.NotifyControllerDidAppear();
            
            NotifySizeChanged();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            _controllersCollectionViewLayout.InvalidateLayout();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (ControllersCollectionView != null)
            {
                if (ControllersCollectionView?.VisibleCells?.Any() ?? false)
                {
                    (ControllersCollectionView.VisibleCells.First() as ControllerCollectionViewCell)
                        ?.NotifyControllerWillDisappear();
                }
            }

            _controllersBoundsChangeObserver?.Dispose();
            _controllersFrameChangeObserver?.Dispose();
        }
        
        private void ControllersCollectionViewBoundsObserver(NSObservedChange obj)
        {
            NotifySizeChanged();
        }

        private void NotifySizeChanged()
        {
            var visibleCells = ControllersCollectionView.VisibleCells;
            if (visibleCells?.Any() ?? false)
            {
                (visibleCells.First() as ControllerCollectionViewCell)?.NotifySizeChanged(
                    ControllersCollectionView.Frame);
            }
        }
        
        public object Selected => TopBarCollectionViewSource?.SelectedItem;
    }
}