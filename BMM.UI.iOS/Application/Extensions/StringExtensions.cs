using System.Globalization;
using BMM.Core.Constants;

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
    }
}