using System;
using CoreGraphics;

namespace BMM.UI.iOS.Extensions
{
    public static class CGSizeExtensions
    {
        public static CGSize WithWidth(this CGSize size, nfloat width)
            => new CGSize(width, size.Height);

        public static CGSize WithHeight(this CGSize size, nfloat height)
            => new CGSize(size.Width, height);

        public static CGSize AddWidth(this CGSize size, nfloat additionalWidth)
            => new CGSize(size.Width + additionalWidth, size.Height);

        public static CGSize AddHeight(this CGSize size, nfloat additionalHeight)
            => new CGSize(size.Width, size.Width + additionalHeight);
    }
}