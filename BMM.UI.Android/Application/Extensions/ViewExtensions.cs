using Android.Views;

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

        public static void UpdateSize(this View view, int size)
        {
            var lp = view.LayoutParameters;

            if (lp!.Height == size && lp.Width == size)
                return;

            lp.Height = size;
            lp.Width = size;
            view.LayoutParameters = lp;
        }
    }
}