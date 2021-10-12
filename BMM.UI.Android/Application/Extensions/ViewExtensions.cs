using Android.Views;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class ViewExtensions
    {
        public static void UpdateHeight(this View view, int height)
        {
            var lp = view.LayoutParameters;

            if (lp!.Height == height)
                return;

            lp.Height = height;
            view.LayoutParameters = lp;
        }
    }
}