using Android.Content;
using Android.Util;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.LayoutManagers.Scrollers;

namespace BMM.UI.Droid.Application.LayoutManagers;

public class CenterLayoutManager : LinearLayoutManager
{
    public CenterLayoutManager(Context context) : base(context)
    {
    }

    public CenterLayoutManager(Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
    {
    }

    public CenterLayoutManager(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
    {
    }

    public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state, int position)
    {
        var centerSmoothScroller = new CenterSmoothScroller(recyclerView.Context);
        centerSmoothScroller.TargetPosition = position;
        StartSmoothScroll(centerSmoothScroller);
    }
}