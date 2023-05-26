using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public class PlayerNavigationController : MvxNavigationController
    {
        private UIColor BarBackgroundColor => AppColors.BackgroundOneColor;
        private UIColor BarForegroundColor => AppColors.LabelOneColor;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                SetNewNavigationBarAppearance();
            else
                SetOldNavigationBarAppearance();

            NavigationBar.TintColor = AppColors.TintColor;
        }

        private void SetNewNavigationBarAppearance()
        {
            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = BarBackgroundColor;
            appearance.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = BarForegroundColor
            };
            NavigationBar.StandardAppearance = appearance;
            NavigationBar.ScrollEdgeAppearance = appearance;
        }

        private void SetOldNavigationBarAppearance()
        {
            NavigationBar.BarTintColor = BarBackgroundColor;
            NavigationBar.Translucent = false;
            NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = BarForegroundColor
            };
        }
    }
}