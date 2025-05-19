using System.Collections.Concurrent;
using BMM.Core.Constants;
using BMM.UI.iOS.Constants;
using FFImageLoading;
using Microsoft.IdentityModel.Tokens;

namespace BMM.UI.iOS.Extensions;

public static class UIImageExtensions
{
    private const int BadgeSize = 8;

    public static UIImage WithBadge(this UIImage image, UIColor iconColor)
    {
        var size = image.Size;
        var renderer = new UIGraphicsImageRenderer(size);

        return renderer.CreateImage(_ =>
            {
                var tintedImage = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                iconColor.GetResolvedColorSafe().SetFill();
                tintedImage.Draw(CGPoint.Empty);

                var badgeSize = new CGSize(BadgeSize, BadgeSize);
                var badgeOrigin = new CGPoint(0, 0);
                var badgeRect = new CGRect(badgeOrigin, badgeSize);

                var badgePath = UIBezierPath.FromOval(badgeRect);
                AppColors.RadioColor.SetFill();
                badgePath.Fill();
            })
            .ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
    }

    public static async Task<UIImage> ToUIImage(this string url)
    {
        try
        {
            if (url.IsNullOrEmpty())
                return UIImage.FromBundle(ImageResourceNames.PlaceholderCover.ToLower());

            return await ImageService
                .Instance
                .LoadUrl(url)
                .AsUIImageAsync();
        }
        catch
        {
            // ignore
            return UIImage.FromBundle(ImageResourceNames.PlaceholderCover.ToLower());
        }
    }

    public static async Task<IDictionary<string, UIImage>> DownloadAsImages(this IEnumerable<string> urls, int maxSecondsToLoad = 5)
    {
        var imagesDictionary = new ConcurrentDictionary<string, UIImage>();

        try
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(maxSecondsToLoad));
            await Parallel.ForEachAsync(urls,
                cancellationTokenSource.Token,
                async (url, token) => { imagesDictionary.TryAdd(url, await url.ToUIImage()); });

            return imagesDictionary;
        }
        catch
        {
            return imagesDictionary.ToDictionary();
        }
    }

    public static UIImage GetCover(this IDictionary<string, UIImage> coversDictionary, string coverUrl)
    {
        try
        {
            if (coverUrl.IsNullOrEmpty() || !coversDictionary.TryGetValue(coverUrl, out UIImage cover))
                return UIImage.FromBundle(ImageResourceNames.PlaceholderCover.ToLower());

            return cover;
        }
        catch
        {
            // ignore
            return UIImage.FromBundle(ImageResourceNames.PlaceholderCover.ToLower());
        }
    }
}