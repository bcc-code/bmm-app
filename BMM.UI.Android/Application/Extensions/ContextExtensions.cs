using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.Content;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class ContextExtensions
    {
        public static Color GetColorFromResource(this Context context, int id)
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.M
                ? new Color(ContextCompat.GetColor(context, id))
                : context.Resources!.GetColor(id);
        }
        
        public static bool IsNightMode(this Context context)
        {
            return context.Resources!.GetString(Resource.String.is_night_mode) == true.ToString().ToLower();
        }
    }
}