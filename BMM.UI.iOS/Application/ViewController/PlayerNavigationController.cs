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
            NavigationBar.BarTintColor = AppColors.PlayerBackgroundColor;
            NavigationBar.Translucent = false;
            NavigationBar.TintColor = AppColors.ColorPrimary;
            NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }
    }
}