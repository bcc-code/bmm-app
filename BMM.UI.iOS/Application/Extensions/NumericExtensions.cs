using System;

namespace BMM.UI.iOS.Extensions
{
    public static class NumericExtensions
    {
        public static float ToRadians(this float val)
        {
            return (float)(Math.PI / 180) * val;
        }
    }
}