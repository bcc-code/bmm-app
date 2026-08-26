using System;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public class ContainmentNavigationViewController : MvxNavigationController, IBaseViewController
    {
        private UIViewController _viewController;

        public Type ParentViewControllerType => typeof(ContainmentViewController);

        public void RegisterViewController(IBaseViewController viewController)
        {
            _viewController = viewController as UIViewController;
            SetViewControllers(new[] {_viewController}, false);
        }

        public bool IsVisible()
        {
            return IsViewLoaded && View.Window != null;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationBar.ApplyBmmAppearance();
            NavigationBar.TintColor = AppColors.TintColor;
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            if (_viewController != null)
            {
                return _viewController.PreferredInterfaceOrientationForPresentation();
            }
            else
            {
                return base.PreferredInterfaceOrientationForPresentation();
            }
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (_viewController != null)
            {
                return _viewController.GetSupportedInterfaceOrientations();
            }
            else
            {
                return base.GetSupportedInterfaceOrientations();
            }
        }
    }
}