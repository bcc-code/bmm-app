using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.Listeners.Interfaces;

namespace BMM.UI.Droid.Application.Listeners
{
    public class SnapOnScrollListener : RecyclerView.OnScrollListener
    {
        private readonly SnapHelper _snapHelper;
        private readonly IRecyclerViewSnapHandler _recyclerViewSnapHandler;
        private int _snapPosition = RecyclerView.NoPosition;

        public SnapOnScrollListener(SnapHelper snapHelper, IRecyclerViewSnapHandler recyclerViewSnapHandler)
        {
            _snapHelper = snapHelper;
            _recyclerViewSnapHandler = recyclerViewSnapHandler;
        }
        
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            NotifySnapPositionChangeIfNeeded(recyclerView);
        }

        private void NotifySnapPositionChangeIfNeeded(RecyclerView recyclerView)
        {
            int snapPosition = GetSnapPosition(_snapHelper, recyclerView);
            bool snapPositionChanged = snapPosition != _snapPosition;
            if (snapPositionChanged)
            {
                _snapPosition = snapPosition;
                _recyclerViewSnapHandler?.OnPositionChanged(_snapPosition);
            }
        }

        private int GetSnapPosition(SnapHelper snapHelper, RecyclerView recyclerView)
        {
            var layoutManager = recyclerView.GetLayoutManager();
            var snapView = snapHelper.FindSnapView(layoutManager);
            return layoutManager!.GetPosition(snapView!);
        }
    }
}