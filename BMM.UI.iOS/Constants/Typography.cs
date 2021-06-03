using System;
using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class Typography
    {
        public static Lazy<UIFont> Title1 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(18, UIFontWeight.Bold);
            return font;
        });

        public static Lazy<UIFont> Header2 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(28, UIFontWeight.Black);
            return font;
        });

        public static Lazy<UIFont> Paragraph2 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(15);
            return font;
        });

        public static Lazy<UIFont> Subtitle3 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(13);
            return font;
        });
    }
}