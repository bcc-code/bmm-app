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

        public static readonly Lazy<UIFont> Title2 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(16, UIFontWeight.Bold);
            return font;
        });

        public static readonly Lazy<UIFont> Title4 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular);
            return font;
        });

        public static readonly Lazy<UIFont> Header2 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(28, UIFontWeight.Heavy);
            return font;
        });

        public static readonly Lazy<UIFont> Header3 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(24, UIFontWeight.Heavy);
            return font;
        });

        public static readonly Lazy<UIFont> Paragraph2 = new Lazy<UIFont>(() =>
        {
            var font = UIFont.SystemFontOfSize(15);
            return font;
        });

        public static readonly Lazy<UIFont> Subtitle3 = new Lazy<UIFont>(() => UIFont.SystemFontOfSize(13));
    }
}