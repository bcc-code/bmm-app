using System.Runtime.Remoting;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using BMM.UI.Droid.Application.Constants.Player;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross;
using MvvmCross.Base;

namespace BMM.UI.Droid.Utils
{
    public class ViewUtils
    {
        public static void SetSpecifiedNavigationBarColor(Activity activity, Color color) 
            => SetNavigationBarColor(activity, color);

        public static void SetDefaultNavigationBarColor(Activity activity) 
            => SetNavigationBarColor(activity, activity.GetColorFromResource(Resource.Color.label_primary_reverted_color));

        private static void SetNavigationBarColor(Activity activity, Color color)
        {
            var sdkVersionHelper = Mvx.IoCProvider.Resolve<ISdkVersionHelper>();
            if (activity?.Window == null || activity?.Resources == null || !sdkVersionHelper.SupportsNavigationBarColors)
                return;
            
            activity.Window.ClearFlags(WindowManagerFlags.TranslucentNavigation);
            activity.Window.SetNavigationBarColor(color);
        }
        
        public static bool IsLong(int width, int height)
        {
            float widthToHeightRatio = width / (float)height;
            return widthToHeightRatio < CoverConstants.WidthToHighRatio.LongThreshold;
        }
    }
}