using System;
using System.Linq;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using BMM.UI.iOS.CustomViews;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.NewMediaPlayer;
using CoreGraphics;
using MvvmCross;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ContainmentViewController : UIViewController, IMvxCanCreateIosView, IBaseViewController
    {
        private UIView _contentView;
        private UIView _miniPlayerView;

        public System.Type ParentViewControllerType => typeof(MenuViewController);

        public ContainmentViewController() : base(nameof(ContainmentViewController), null)
        { }

        public IBaseViewController EnclosedViewController { get; set; }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Show the mini-player if there are items in the queue
            if (Mvx.IoCProvider.Resolve<IMediaPlayer>().CurrentTrack != null)
            {
                RegisterViewController(this.CreateViewControllerFor<MiniPlayerViewModel>() as MiniPlayerViewController);
            }

            if (_contentView != null)
            {
                SetContent(_contentView);
                _contentView = null;
            }

            if (_miniPlayerView != null)
            {
                SetMiniPlayerView(_miniPlayerView);
                _miniPlayerView = null;
            }

            //TODO Show Badge on bottom bar
            //TabBarItem.Image = TabBarItem.Image.WithBadge();
            //TabBarItem.SelectedImage = TabBarItem.SelectedImage.WithBadge();
        }

        public void RegisterViewController(IBaseViewController viewController)
        {
            if (viewController as MiniPlayerViewController == null && viewController as ContainmentNavigationViewController == null)
            {
                throw new NotSupportedException("The ContainmentViewController has no defined place where it can show the viewController '" + viewController.GetType().Name +
                                                "'. I guess, the ContainmentNavigationViewController was meant as parent instead of the ContainmentViewController.");
            }

            var _viewController = viewController as UIViewController;

            AddChildViewController(_viewController);

            if (viewController is MiniPlayerViewController)
            {
                // The MiniPlayer takes position at the lower end of the ContainmentController. It thereby sticks there on every view, that's changed in the UINavigationController.
                if (MiniplayerView == null)
                {
                    _miniPlayerView = _viewController.View;
                }
                else
                {
                    SetMiniPlayerView(_viewController.View);
                }
            }

            if (viewController is ContainmentNavigationViewController)
            {
                if (ContentView == null)
                {
                    _contentView = _viewController.View;
                }
                else
                {
                    SetContent(_viewController.View);
                }
            }

            _viewController.DidMoveToParentViewController(this);
        }

        private void SetContent(UIView view)
        {
            // Set the bounds and frame to what we have in the ContentView. This sets the UIView to the same size-settings, the content-view has.
            view.Frame = ContentView.Frame;

            ContentView.AddSubview(view);
        }

        private void SetMiniPlayerView(UIView view)
        {
            // We don't need the mini-player twice ...
            if (MiniplayerView.Subviews.Length > 0)
                return;

            // the view of MiniPlayerViewController is generated and creates its own constraints
            view.TranslatesAutoresizingMaskIntoConstraints = false;
            MiniplayerView.AddSubview(view);

            // Update the height of the view to the boundary-height of the miniplayer-view that's up to be inserted here.
            foreach (NSLayoutConstraint constraint in MiniplayerView.Constraints)
            {
                if (constraint.FirstAttribute == NSLayoutAttribute.Height)
                {
                    constraint.Constant = view.Bounds.Height;
                    break;
                }
            }

            view.LeadingAnchor.ConstraintEqualTo(MiniplayerView.LeadingAnchor).Active = true;
            view.TrailingAnchor.ConstraintEqualTo(MiniplayerView.TrailingAnchor).Active = true;
            view.TopAnchor.ConstraintEqualTo(MiniplayerView.TopAnchor).Active = true;
            view.BottomAnchor.ConstraintEqualTo(MiniplayerView.BottomAnchor).Active = true;
        }

        public bool IsVisible()
        {
            return IsViewLoaded && View.Window != null;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return VersionHelper.SupportsDarkMode ? UIStatusBarStyle.DarkContent : UIStatusBarStyle.Default;
        }

        public ContainmentNavigationViewController ContainmentNavigationViewController => ChildViewControllers.FirstOrDefault() as ContainmentNavigationViewController;

        public IMvxViewModel NavigationRootViewModel
        {
            get
            {
                var maxTopController = ContainmentNavigationViewController?.TopViewController as MvxViewController;
                return maxTopController?.ViewModel;
            }
        }

        public bool HasNavigationStack => ContainmentNavigationViewController?.ViewControllers?.Length > 1;

        public void PopToRootViewController(bool animated)
        {
            ContainmentNavigationViewController?.PopToRootViewController(animated);
        }

        public void ScrollToTop()
        {
            var navigation = ContainmentNavigationViewController;
            var rootViewController = navigation?.ViewControllers?.FirstOrDefault();

            if (navigation == null || rootViewController?.View == null)
                return;

            foreach (var viewSubview in rootViewController.View.Subviews)
            {
                if (viewSubview is UIScrollView scrollView)
                {
                    if (rootViewController is IHaveLargeTitle largeTitleViewController)
                    {
                        scrollView.SetContentOffset(new CGPoint(0, -largeTitleViewController.InitialLargeTitleHeight ?? 0), true);

                        return;
                    }

                    scrollView.ScrollRectToVisible(new CGRect(0, 0, 1, 1), true);
                    return;
                }

                if (rootViewController is TTabbedViewController tabbedViewController)
                {
                    var tabScrollViews = tabbedViewController
                        .TabViewControllers
                        .SelectMany(x => x.View.Subviews)
                        .OfType<UIScrollView>();

                    foreach (var tabScrollView in tabScrollViews)
                    {
                        tabScrollView.ScrollRectToVisible(new CGRect(0, 0, 1, 1), true);
                    }
                }
            }
        }
    }
}