using Android.Content.Res;
using Android.Graphics;

namespace BMM.UI.Droid.Application.Bindings
{
    public static class ColorStateListHelper{
        public static ColorStateList ParseString(string colorString)
        {
            if (colorString == null)
                return null;

            int[][] states = { new int[] { } };
            var color = new int[]
            {
                Color.ParseColor(colorString)
            };
            return new ColorStateList(states, color);
        }
    }
}