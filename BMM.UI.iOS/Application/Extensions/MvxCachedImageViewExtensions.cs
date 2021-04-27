using BMM.UI.iOS.Helpers;
using FFImageLoading.Cross;

namespace BMM.UI.iOS.Extensions
{
    public static class MvxCachedImageViewExtensions
    {
        public static void ErrorAndLoadingPlaceholderImagePath(this MvxCachedImageView imageView, string imagePath)
        {
            imageView.ErrorPlaceholderImagePath = imagePath;
            imageView.LoadingPlaceholderImagePath = imagePath;
        }

        public static void ErrorAndLoadingPlaceholderImagePathForCover(this MvxCachedImageView imageView)
        {
            imageView.ErrorAndLoadingPlaceholderImagePath(IosConstants.CoverPlaceholderImage);
        }
    }
}