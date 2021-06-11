﻿using System;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Implementations;
using BMM.Core.ViewModels;
using BMM.UI.iOS.NewMediaPlayer;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    public class Presenter : MvxIosViewPresenter, IViewModelAwareViewPresenter
    {
        public Presenter(IUIApplicationDelegate applicationDelegate, UIWindow window) : base(applicationDelegate, window)
        { }

        public override Task<bool> Show(MvxViewModelRequest request)
        {
            if (request.ViewModelType == typeof(MiniPlayerViewModel))
            {
                var tabBarViewController = TabBarViewController as MenuViewController;
                if (tabBarViewController == null)
                    return Task.FromResult(false);

                foreach (var childViewController in tabBarViewController.ChildViewControllers.OfType<ContainmentViewController>())
                {
                    // we create a view controller for each tab because we can't attach the same UIViewController so multiple ViewController
                    var view = this.CreateViewControllerFor(request);
                    childViewController.RegisterViewController(view as IBaseViewController);
                }

                return Task.FromResult(true);
            }

            return base.Show(request);
        }

        protected override async Task<bool> ShowChildViewController(UIViewController viewController, MvxChildPresentationAttribute attribute, MvxViewModelRequest request)
        {
            if (!ModalViewControllers.Any())
                return await base.ShowChildViewController(viewController, attribute, request);

            if (ModalViewControllers.FirstOrDefault() is UINavigationController navigationController && !(viewController is QueueViewController))
            {
                if (navigationController.ChildViewControllers.FirstOrDefault() is PlayerViewController)
                    await CloseModalViewControllers();
            }

            return await base.ShowChildViewController(viewController, attribute, request);
        }

        protected override Task<bool> ShowTabViewController(UIViewController viewController, MvxTabPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var containmentViewController = Activator.CreateInstance<ContainmentViewController>();
            var containmentNavigationController = Activator.CreateInstance<ContainmentNavigationViewController>();
            containmentViewController.RegisterViewController(containmentNavigationController);
            containmentNavigationController.RegisterViewController(viewController as IBaseViewController);
            if (VersionHelper.SupportsLargeTitles)
                containmentNavigationController.NavigationBar.PrefersLargeTitles = true;
            return base.ShowTabViewController(containmentViewController, attribute, request);
        }

        protected override MvxNavigationController CreateNavigationController(UIViewController viewController)
        {
            if (viewController is PlayerViewController)
            {
                var playerNav = Activator.CreateInstance<PlayerNavigationController>();
                playerNav.SetViewControllers(new []{ viewController}, true);
                return playerNav;
            }

            var navController = new ContainmentNavigationViewController();
            navController.RegisterViewController(viewController as IBaseViewController);
            return navController;
        }

        public UIViewController CurrentRootViewController => Window.RootViewController?.ModalViewController is PlayerNavigationController
            ? Window.RootViewController.ModalViewController
            : Window.RootViewController;

        public bool IsViewModelShown<T>()
        {
            var vmType = typeof(T);
            var containmentNavigationController = GetContainmentNavigationControllerOfSelectedTab();

            var mvxTopController = containmentNavigationController?.TopViewController as MvxViewController;
            if (mvxTopController == null)
                return false;

            return mvxTopController.ViewModel?.GetType() == vmType;
        }

        public bool IsViewModelInStack<T>()
        {
            var vmType = typeof(T);
            var containmentNavigationController = GetContainmentNavigationControllerOfSelectedTab();
            if (containmentNavigationController == null)
                return false;

            return containmentNavigationController.ChildViewControllers.OfType<MvxViewController>().Any(controller => controller.ViewModel?.GetType() == vmType);
        }

        private ContainmentNavigationViewController GetContainmentNavigationControllerOfSelectedTab()
        {
            if (TabBarViewController is UITabBarController tabBarController)
            {
                var selectedViewController = tabBarController.SelectedViewController as ContainmentViewController;

                var navController = selectedViewController?.ChildViewControllers.FirstOrDefault(vc => vc is ContainmentNavigationViewController);
                return navController as ContainmentNavigationViewController;
            }

            return null;
        }
    }
}