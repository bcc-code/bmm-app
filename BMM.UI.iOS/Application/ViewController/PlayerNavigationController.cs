using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public class PlayerNavigationController: MvxNavigationController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = AppColors.PlayerBackgroundColor;
            appearance.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };
            NavigationBar.StandardAppearance = appearance;
            NavigationBar.ScrollEdgeAppearance = appearance;
            NavigationBar.TintColor = AppColors.ColorPrimary;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }
    }
}