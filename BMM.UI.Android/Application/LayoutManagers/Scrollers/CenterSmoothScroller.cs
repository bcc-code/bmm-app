using Android.Content;
using Android.Util;
using AndroidX.RecyclerView.Widget;

namespace BMM.UI.Droid.Application.LayoutManagers.Scrollers;

public class CenterSmoothScroller : LinearSmoothScroller
{
    private const float MillisecondsPerInch = 100f;
            
    public CenterSmoothScroller(Context context) : base(context)
    {
    }

    protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
    {
        return MillisecondsPerInch / (int)displayMetrics!.DensityDpi;
    }

    public override int CalculateDtToFit(int viewStart, int viewEnd, int boxStart, int boxEnd, int snapPreference)
    {
        return (boxStart + (boxEnd - boxStart) / 2) - (viewStart + (viewEnd - viewStart) / 2);
    }
}