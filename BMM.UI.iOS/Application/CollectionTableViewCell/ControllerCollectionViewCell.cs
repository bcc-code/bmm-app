using System;
using System.Linq;
using BMM.UI.iOS.CollectionViewSource;
using BMM.UI.iOS.Delegates;
using CoreGraphics;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS.CollectionTableViewCell
{
    public partial class ControllerCollectionViewCell
        : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ControllerCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(ControllerCollectionViewCell), NSBundle.MainBundle);

        private object _item;
        private CGRect _lastRect;
        private bool _alreadyInitialized;
        private MvxViewController _controller;
        private IDisposable _frameChangeObserver;
        private IDisposable _boundsChangeObserver;
        private bool _shouldRefreshViewController;
        private bool _shouldSkipFirstViewWillAppear = true;

        protected ControllerCollectionViewCell(IntPtr handle) : base(handle)
        {
        }

        private CreateOrRefreshViewControllerDelegate CreateOrRefreshViewController { get; set; }
        private ControllersCollectionViewSource CollectionViewSource { get; set; }

        public MvxViewController Controller
        {
            get => _controller;
            private set
            {
                if (ReferenceEquals(_controller, value))
                    return;

                _controller = value;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            if (_controller is null || ControllerContainer.Subviews.Contains(_controller.View))
                return;

            foreach (var subview in ControllerContainer.Subviews)
                subview.RemoveFromSuperview();

            ControllerContainer.Add(_controller.View);
            _controller.View.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
        }

        private object Item
        {
            get => _item;
            set
            {
                if (ReferenceEquals(_item, value))
                    return;

                _item = value;
                _shouldRefreshViewController = true;
            }
        }

        public void InitializeCell(
            ControllersCollectionViewSource controllersCollectionViewSource,
            CreateOrRefreshViewControllerDelegate createOrRefreshViewController,
            object item)
        {
            Item = item;

            if (_alreadyInitialized)
                return;

            _alreadyInitialized = true;
            CollectionViewSource = controllersCollectionViewSource;
            CollectionViewSource.ScrollAnimationEndedEvent += OnCollectionViewSourceOnScrollAnimationEnded;
            CreateOrRefreshViewController = createOrRefreshViewController;
        }

        private void OnCollectionViewSourceOnScrollAnimationEnded(object sender, EventArgs e)
        {
            RefreshViewControllerIfNecessary();
        }

        public void NotifyControllerWillAppear()
        {
            RefreshViewControllerIfNecessary();
            UpdateView();
            if (_shouldSkipFirstViewWillAppear)
            {
                _shouldSkipFirstViewWillAppear = false;
                return;
            }

            Controller?.ViewWillAppear(false);

            if (_boundsChangeObserver != null)
                return;

            _boundsChangeObserver = AddObserver(
                "bounds",
                NSKeyValueObservingOptions.New,
                ControllersCollectionViewBoundsObserver);

            _frameChangeObserver = AddObserver(
                "frame",
                NSKeyValueObservingOptions.New,
                ControllersCollectionViewBoundsObserver);
        }

        public void NotifyControllerDidAppear()
        {
            Controller?.ViewDidAppear(false);
        }

        public void NotifyControllerWillDisappear()
        {
            Controller?.ViewWillDisappear(false);
            Controller?.View?.RemoveFromSuperview();
            _boundsChangeObserver?.Dispose();
            _frameChangeObserver?.Dispose();

            _boundsChangeObserver = null;
            _frameChangeObserver = null;
        }

        private void RefreshViewControllerIfNecessary()
        {
            if (!_shouldRefreshViewController)
                return;

            _shouldRefreshViewController = false;
            Controller = CreateOrRefreshViewController(Item, Controller);
        }

        private void ControllersCollectionViewBoundsObserver(NSObservedChange obj)
        {
            ChangeRect();
        }

        private void ChangeRect()
        {
            Frame = new CGRect(Frame.X, 0, _lastRect.Width, _lastRect.Height);
            Controller.View.Frame = new CGRect(0, 0, _lastRect.Width, _lastRect.Height);
            LayoutIfNeeded();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Frame.Y != 0)
            {
                var width = _lastRect.Width == 0 ? Frame.Width : _lastRect.Width;
                var height = _lastRect.Height == 0 ? Frame.Height : _lastRect.Height;
                Frame = new CGRect(Frame.X, 0, width, height);;
            }
        }

        public void NotifySizeChanged(CGRect containerRect)
        {
            _lastRect = containerRect;
            ChangeRect();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;

            var collectionViewSource = CollectionViewSource;
            if (collectionViewSource != null)
                collectionViewSource.ScrollAnimationEndedEvent -= OnCollectionViewSourceOnScrollAnimationEnded;
        }
    }
}