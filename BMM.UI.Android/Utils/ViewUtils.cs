using System.Runtime.Remoting;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross;
using MvvmCross.Base;

namespace BMM.UI.Droid.Utils
{
    public class ViewUtils
    {
        public static Drawable GenerateBackgroundWithShadow(View view,
            int cornerRadius,
            Color shadowColor,
            int elevation)
        {
            float[] outerRadius =
            {
                cornerRadius, cornerRadius, cornerRadius,
                cornerRadius, cornerRadius, cornerRadius, cornerRadius,
                cornerRadius
            };

            Paint backgroundPaint = new Paint();
            backgroundPaint.SetStyle(Paint.Style.Fill);
            backgroundPaint.SetShadowLayer(cornerRadius,
                0,
                0,
                0);

            var shapeDrawablePadding = new Rect();
            shapeDrawablePadding.Left = elevation;
            shapeDrawablePadding.Right = elevation;
            shapeDrawablePadding.Top = elevation;
            shapeDrawablePadding.Bottom = elevation;

            var shapeDrawable = new ShapeDrawable();
            shapeDrawable.SetPadding(shapeDrawablePadding);

            shapeDrawable.Paint.Color = Color.Gray;
            shapeDrawable.Paint
            .SetShadowLayer(cornerRadius / 3,
                0,
                0,
                shadowColor);

            view.SetLayerType(LayerType.Software, shapeDrawable.Paint);

            shapeDrawable.Shape = new RoundRectShape(outerRadius, null, null);

            LayerDrawable drawable = new LayerDrawable(new Drawable[] { shapeDrawable });
            drawable.SetLayerInset(0,
                elevation,
                elevation * 2,
                elevation,
                elevation * 2);

            return drawable;
        }
        
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
    }
}