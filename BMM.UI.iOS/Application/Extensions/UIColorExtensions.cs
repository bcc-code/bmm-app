using System;
using BMM.UI.iOS.NewMediaPlayer;
using BMM.UI.iOS.Utils.ColorPalette;
using UIKit;

namespace BMM.UI.iOS.Extensions
{
    public static class UIColorExtensions
    {
        public static YUVColor ToYUV(this UIColor color)
        {
            var cgColor = color.CGColor;

            float[] rgb = { 0, 0, 0 };

            float y = 0;
            float u = 0;
            float v = 0;

            for (int i = 0; i < cgColor.NumberOfComponents.ToInt32() - 1; i++)
                rgb[i] = (float)cgColor.Components[i];

            y = 0.299f * rgb[0] + 0.587f * rgb[1] + 0.114f * rgb[2];
            u = 0.492f * (rgb[2] - y);
            v = 0.877f * (rgb[0] - y);

            return new YUVColor() { Y = y, U = u, V = v };
        }
        
        public static UIColor Blend(this UIColor color1, UIColor color2, float intensity1 = 0.5f, float intensity2 = 0.5f)
        {
            float total = intensity1 + intensity2;
            
            float l1 = intensity1 / total;
            float l2 = intensity2 / total;

            (nfloat R, nfloat G, nfloat B, nfloat A) colorsOne = (R: 0f, G: 0f, B: 0f, A: 0f);
            (nfloat R, nfloat G, nfloat B, nfloat A) colorsTwo = (R: 0f, G: 0f, B: 0f, A: 0f);

            color1.GetRGBA(out colorsOne.R, out colorsOne.G, out colorsOne.B, out colorsOne.A);
            color2.GetRGBA(out colorsTwo.R, out colorsTwo.G, out colorsTwo.B, out colorsTwo.A);

            return new UIColor(
                l1 * colorsOne.R + l2 * colorsTwo.R,
                l1 * colorsOne.G + l2 * colorsTwo.G,
                l1 * colorsOne.B + l2 * colorsTwo.B,
                l1 * colorsOne.A + l2 * colorsTwo.A);
        }
        
        /// <summary>
        /// Save to use with iOS version below 13.0
        /// </summary>
        public static UIColor GetResolvedColorSafe(this UIColor color, UIUserInterfaceStyle userInterfaceStyle)
        {
            if (VersionHelper.SupportsDarkMode)
                return color.GetResolvedColor(UITraitCollection.FromUserInterfaceStyle(userInterfaceStyle));

            return color;
        }
    }
}