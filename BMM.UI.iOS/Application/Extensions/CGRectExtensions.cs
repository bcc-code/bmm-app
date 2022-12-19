using System;
using CoreGraphics;

namespace BMM.UI.iOS.Extensions
{
    public static class CGRectExtensions
    {
        public static CGRect WithWidth(this CGRect rect, nfloat width)
            => new CGRect(rect.X, rect.Y, width, rect.Height);

        public static CGRect WithHeight(this CGRect rect, nfloat height)
            => new CGRect(rect.X, rect.Y, rect.Width, height);

        public static CGRect WithX(this CGRect rect, nfloat x)
            => new CGRect(x, rect.Y, rect.Width, rect.Height);

        public static CGRect WithY(this CGRect rect, nfloat y)
            => new CGRect(rect.X, y, rect.Width, rect.Height);

        public static CGRect WithPosition(this CGRect rect, nfloat x, nfloat y)
            => new CGRect(x, y, rect.Width, rect.Height);

        public static CGRect WithSize(this CGRect rect, nfloat width, nfloat height)
            => new CGRect(rect.X, rect.Y, width, height);
    }
}