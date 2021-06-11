using System;
using UIKit;

namespace BMM.UI.iOS.Constants
{
    /// <summary>
    /// Font weight mapping for system font
    /// 400 -> Regular
    /// 500 -> Medium
    /// 600 -> Semibold
    /// 700 -> Bold
    /// 800 -> Heavy
    /// 900 -> Black
    /// </summary>
    public static class Typography
    {
        public static readonly Lazy<UIFont> Title1 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(18, UIFontWeight.Bold);
            return font;
        });

        public static readonly Lazy<UIFont> Header2 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(28, UIFontWeight.Heavy);
            return font;
        });

        public static readonly Lazy<UIFont> Paragraph2 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(15);
            return font;
        });
    }
}