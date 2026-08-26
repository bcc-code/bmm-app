using BMM.UI.iOS.Constants;
using UIKit;

namespace BMM.UI.iOS.Extensions
{
    public static class UINavigationBarExtensions
    {
        /// <summary>
        /// Applies the standard BMM navigation bar appearance.
        /// The title colors have to be set explicitly. Starting with iOS 26 the bars are rendered with Liquid Glass,
        /// which no longer derives the title color from the background color we configure here. Without an explicit
        /// <see cref="UINavigationBarAppearance.LargeTitleTextAttributes"/> the large title becomes unreadable in dark mode.
        /// </summary>
        public static void ApplyBmmAppearance(this UINavigationBar navigationBar)
        {
            var titleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = AppColors.LabelOneColor
            };

            var standardAppearance = new UINavigationBarAppearance();
            standardAppearance.ConfigureWithOpaqueBackground();
            standardAppearance.BackgroundColor = AppColors.BackgroundOneColor;
            standardAppearance.TitleTextAttributes = titleTextAttributes;
            standardAppearance.LargeTitleTextAttributes = titleTextAttributes;
            standardAppearance.ShadowColor = AppColors.SeparatorColor;

            var scrollEdgeAppearance = (UINavigationBarAppearance)standardAppearance.Copy();
            scrollEdgeAppearance.ShadowColor = UIColor.Clear;

            navigationBar.StandardAppearance = standardAppearance;
            navigationBar.ScrollEdgeAppearance = scrollEdgeAppearance;
        }
    }
}
