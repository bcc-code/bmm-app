using Android.Views;
using BMM.Core.Constants;
using BMM.UI.Droid.Application.Constants.Player;

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
        
        public static bool IsLong(this View view)
        {
            float widthToHeightRatio = view.Width / (float)view.Height;
            return widthToHeightRatio < CoverConstants.WidthToHighRatio.LongThreshold;
        }
    }
}