using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using BMM.Core.Constants;
using BMM.UI.Droid.Application.Constants.Player;
using BMM.UI.Droid.Utils;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class ViewExtensions
    {
        public static void UpdateWidth(this View view, int width)
        {
            var lp = view.LayoutParameters;

            if (lp!.Width == width)
                return;

            lp.Width = width;
            view.LayoutParameters = lp;
        }
        
        public static void UpdateHeight(this View view, int height)
        {
            var lp = view.LayoutParameters;

            if (lp!.Height == height)
                return;

            lp.Height = height;
            view.LayoutParameters = lp;
        }

        public static void UpdateSize(this View view, int size)
        {
            var lp = view.LayoutParameters;

            if (lp!.Height == size && lp.Width == size)
                return;

            lp.Height = size;
            lp.Width = size;
            view.LayoutParameters = lp;
        }
        
        public static bool IsPortrait(this View view)
        {
            return ViewUtils.IsPortrait(view.Width, view.Height);
        }  
        
        public static bool HasChild(this ViewGroup view, View childToCheck)
        {
            for (int i = 0; i < view.ChildCount; i++)
            {
                var child = view.GetChildAt(i);
                
                if (child == childToCheck)
                    return true;
            }

            return false;
        }
        
        public static void ApplyRoundedCorners(
            this View view, 
            float topLeftRadius,
            float bottomLeftRadius,
            float topRightRadius, 
            float bottomRightRadius)
        {
            var backgroundColor = Color.Transparent;
            
            if (view.Background is ColorDrawable colorDrawable)
                backgroundColor = colorDrawable.Color;
            
            float[] radii =
            [
                topLeftRadius, topLeftRadius,
                topRightRadius, topRightRadius,
                bottomRightRadius, bottomRightRadius,
                bottomLeftRadius, bottomLeftRadius
            ];

            var drawable = new GradientDrawable();
            drawable.SetShape(ShapeType.Rectangle);
            drawable.SetCornerRadii(radii);
            drawable.SetColor(backgroundColor); 
            
            view.Background = drawable;
        }
    }
}