using System.Globalization;
using System.Linq;
using BMM.Core.Constants;
using BMM.UI.iOS.NewMediaPlayer;

namespace BMM.UI.iOS.Extensions
{
    public static class StringExtensions
    {
        public static string ToIosImageName(this string imagePath)
        {
            var textInfo = CultureInfo.InvariantCulture.TextInfo;

            string underscoreRemovedString = imagePath.Replace(StringConstants.Underscore, StringConstants.Space);

            return textInfo
                .ToTitleCase(underscoreRemovedString)
                .Replace(StringConstants.Space, string.Empty);
        }
        
        public static string ToStandardIosImageName(this string imagePath)
        {
            return imagePath?.Replace(StringConstants.Underscore, string.Empty);
        }
        
        public static long GetCachePlayerItemExpectedSize(this string filePath)
        {
            string sizeString = filePath
                .Split(StringConstants.Underscore)
                .Last()
                .Replace(CacheMediaFileHandle.Extension, string.Empty);

            if (long.TryParse(sizeString, out long result))
                return result;

            return NumericConstants.Zero;
        }
        
        public static long GetCachePlayerItemCreatedTime(this string filePath)
        {
            string createdUnixMs = filePath
                .Split(StringConstants.Underscore)[1];

            if (long.TryParse(createdUnixMs, out long result))
                return result;

            return NumericConstants.Zero;
        }
    }
}