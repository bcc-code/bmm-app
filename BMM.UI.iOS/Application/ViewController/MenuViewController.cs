using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxRootPresentation(WrapInNavigationController = false)]
    public class MenuViewController : MvxTabBarViewController<MenuViewModel>
    {
        public static readonly UIColor SelectedItemTintColor = AppColors.LabelOneColor;
        public static readonly UIColor UnselectedItemTintColor = AppColors.LabelThreeColor;
        
        private const int TimeForLoadingMenuViewControllersInMillis = 500;
        
        public static readonly NSString MenuLoadedNotification = new NSString($"{nameof(MenuViewController)}.MenuLoaded");
        private readonly NSNotificationCenter _notificationCenter = NSNotificationCenter.DefaultCenter;
        private readonly List<string> _translationLabels = new List<string>();
        private bool _tabInitialized;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Delegate = new MenuVTabBarDelegate();
            View.BackgroundColor = AppColors.BackgroundOneColor;
            TabBar.BarTintColor = AppColors.BackgroundOneColor;
            TabBar.TintColor = SelectedItemTintColor;
            TabBar.UnselectedItemTintColor = UnselectedItemTintColor;
            TabBar.AccessibilityIdentifier = "tab_bar";
            SetBottomBarAppearance();
        }

        public override UIViewController SelectedViewController
        {
            get => base.SelectedViewController;
            set
            {
                base.SelectedViewController = value;
                string viewModelName = ((ContainmentViewController)SelectedViewController)
                    .EnclosedViewController
                    .ViewModelName;
                ViewModel.LogBottomBarButtonClicked(viewModelName);
            }
        }
        
        protected virtual void SetBottomBarAppearance()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                return;

            var appearance = new UITabBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.StackedLayoutAppearance.Selected.IconColor = AppColors.LabelOneColor;
            appearance.StackedLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = AppColors.LabelOneColor
            };
            appearance.StackedLayoutAppearance.Normal.IconColor = AppColors.LabelThreeColor;
            appearance.StackedLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = AppColors.LabelThreeColor
            };
            appearance.BackgroundColor = AppColors.BackgroundOneColor;
            TabBar.StandardAppearance = appearance;
            
            if (!UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
                return;

            TabBar.ScrollEdgeAppearance = appearance;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            BottomBarHeight = TabBar.Frame.Height;
        }

        public static nfloat BottomBarHeight { get; private set; }

        protected override void SetTitleAndTabBarItem(UIViewController viewController, MvxTabPresentationAttribute attribute)
        {
            _translationLabels.Add(attribute.TabName);
            attribute.TabName = ViewModel.TextSource.GetText(attribute.TabName);
            base.SetTitleAndTabBarItem(viewController, attribute);
        }

        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel.PropertyChanged += OnTextSourceChanged;

            if (_tabInitialized)
                return;
            
            foreach (var navigationCommand in ViewModel.NavigationCommands)
                navigationCommand.Value.Execute();
                
            _tabInitialized = true;

            _notificationCenter.PostNotificationName(MenuLoadedNotification, null);
            ApplicationStateWatcher.ApplicationStateChanged += ApplicationStateChanged;

            await Task.Delay(TimeForLoadingMenuViewControllersInMillis);
            Mvx.IoCProvider.Resolve<IDeepLinkHandler>().SetReadyToOpenDeepLinkAndHandlePending();
        }

        private void ApplicationStateChanged(ApplicationState applicationState)
        {
            if (applicationState == ApplicationState.Background)
                return;
            
            var containmentController = SelectedViewController as ContainmentViewController;
            if (containmentController?.NavigationRootViewModel is DocumentsViewModel documentsViewModel)
                documentsViewModel.RefreshInBackground();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewModel.PropertyChanged -= OnTextSourceChanged;
        }

        public override bool ShowChildView(UIViewController viewController)
        {
            if (SelectedViewController?.ChildViewControllers.FirstOrDefault() is ContainmentNavigationViewController containmentNavigationViewController)
            {
                containmentNavigationViewController.PushViewController(viewController, true);
                return true;
            }

            return false;
        }

        public override bool CloseChildViewModel(IMvxViewModel viewModel)
        {
            if (SelectedViewController?.ChildViewControllers.FirstOrDefault() is ContainmentNavigationViewController containmentNavigationViewController)
            {
                containmentNavigationViewController.PopViewController(true);
                return true;
            }

            return false;
        }

        public override bool CanShowChildView()
        {
            return SelectedViewController?.ChildViewControllers.FirstOrDefault() is ContainmentNavigationViewController;
        }

        private void OnTextSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "TextSource")
                return;

            UpdateTranslations();
        }

        private void UpdateTranslations()
        {
            if (ViewControllers == null)
                return;

            if (ViewControllers.Length != _translationLabels.Count)
                return;
            for (var i = 0; i < ViewControllers.Length; i++)
            {
                var vc = ViewControllers[i];
                var translationLabel = _translationLabels[i];
                vc.TabBarItem.Title = ViewModel.TextSource.GetText(translationLabel);
            }
        }
    }

    public class MenuVTabBarDelegate : UITabBarControllerDelegate
    {
        public override void ViewControllerSelected(UITabBarController tabBarController, UIViewController viewController)
        {
            var containmentController = viewController as ContainmentViewController;
            if (containmentController?.NavigationRootViewModel is DocumentsViewModel documentsViewModel and not ContentBaseViewModel)
                documentsViewModel.RefreshInBackground();
        }

        public override bool ShouldSelectViewController(UITabBarController tabBarController, UIViewController viewController)
        {
            if (tabBarController.SelectedViewController is ContainmentViewController selectedVc
                && viewController is ContainmentViewController newMvxVc)
            {
                bool isAlreadySelected = selectedVc.NavigationRootViewModel?.GetType() == newMvxVc?.NavigationRootViewModel?.GetType();

                if (!isAlreadySelected)
                    return true;

                if (selectedVc.HasNavigationStack)
                    selectedVc.PopToRootViewController(true);
                else
                    selectedVc.ScrollToTop();

                return false;
            }

            return true;
        }
    }
}