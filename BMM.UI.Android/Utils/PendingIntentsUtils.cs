using Android.App;
using Android.OS;

namespace BMM.UI.Droid.Utils
{
    public static class PendingIntentsUtils
    {
        public static PendingIntentFlags GetImmutable()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                return PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent;

            return PendingIntentFlags.UpdateCurrent;
        }
    }
}