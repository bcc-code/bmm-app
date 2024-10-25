using System;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.NewMediaPlayer;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public class ContainmentNavigationViewController : MvxNavigationController, IBaseViewController
    {
        private UIViewController _viewController;

        public Type ParentViewControllerType => typeof(ContainmentViewController);
        public ContainmentViewController ContainmentVC { get; set; }

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
            if (VersionHelper.SupportsBarAppearanceProxy)
            {
                var standardAppearance = new UINavigationBarAppearance
                {
                    BackgroundColor = UIColor.White.ColorWithAlpha(0.7f),
                    BackgroundEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light),
                };

                var scrollEdgeAppearance = standardAppearance.Copy() as UINavigationBarAppearance;
                scrollEdgeAppearance.ShadowColor = UIColor.Clear;
                NavigationBar.StandardAppearance = standardAppearance;
                NavigationBar.ScrollEdgeAppearance = scrollEdgeAppearance;
            }
            else
            {
                NavigationBar.BarTintColor = UIColor.White.ColorWithAlpha(0.1f);
                NavigationBar.ShadowImage = new UIImage();
            }

            NavigationBar.TintColor = AppColors.TintColor;
            NavigationBar.Translucent = true;
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