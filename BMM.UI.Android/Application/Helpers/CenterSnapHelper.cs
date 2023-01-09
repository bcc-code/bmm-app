using System;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace BMM.UI.Droid.Application.Helpers
{
    public class CenterSnapHelper : LinearSnapHelper
    {
        private bool _scrolled;
        private RecyclerView _recyclerView;

        public override void AttachToRecyclerView(RecyclerView recyclerView)
        {
            _recyclerView = recyclerView;
            recyclerView.ScrollChange += RecyclerViewOnScrollChange;
            base.AttachToRecyclerView(recyclerView);
        }

        public void DetachFromRecyclerView(RecyclerView recyclerView)
        {
            _recyclerView = null;
            base.AttachToRecyclerView(null);
        }

        private void RecyclerViewOnScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            if (!(sender is RecyclerView recyclerView))
                return;

            if (recyclerView.ScrollState == RecyclerView.ScrollStateIdle && _scrolled)
            {
                if (recyclerView.GetLayoutManager() != null)
                {
                    var view = FindSnapView(recyclerView.GetLayoutManager());
                    if (view != null)
                    {
                        var tab = CalculateDistanceToFinalSnap(recyclerView.GetLayoutManager(), view);
                        if (tab != null)
                        {
                            recyclerView.SmoothScrollBy(tab[0], tab[1]);
                        }
                    }
                }

                _scrolled = false;
            }
            else
            {
                _scrolled = true;
            }
        }

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
        {
            if (layoutManager == null)
            {
                return null;
            }

            return FindCenterView(layoutManager, OrientationHelper.CreateHorizontalHelper(layoutManager));
        }

        private View FindCenterView(RecyclerView.LayoutManager layoutManager, OrientationHelper orientationHelper)
        {
            var linearLayoutManager = (LinearLayoutManager)layoutManager;
            int childCount = layoutManager.ChildCount;

            int firstVisibleViewPosition = linearLayoutManager.FindFirstCompletelyVisibleItemPosition();
            int lastVisibleViewPosition = linearLayoutManager.FindLastCompletelyVisibleItemPosition();

            if (firstVisibleViewPosition == 0
                || lastVisibleViewPosition == _recyclerView.GetAdapter().ItemCount - 1)
            {
                return null;
            }

            if (childCount == 0)
                return null;

            View closestChild = null;

            int center = orientationHelper.End / 2;
            int absClosest = int.MaxValue;

            for (int i = 0; i < childCount; i++)
            {
                var child = layoutManager.GetChildAt(i);
                int childCenter = (int)(child.GetX() + child.Width / 2);

                int absDistance = Math.Abs(childCenter - center);

                if (absDistance < absClosest)
                {
                    absClosest = absDistance;
                    closestChild = child;
                }
            }

            return closestChild;
        }

        public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, View targetView)
        {
            var tab = new int[2];
            tab[0] = DistanceToCenter(
                layoutManager,
                targetView,
                OrientationHelper.CreateHorizontalHelper(layoutManager));
            return tab;
        }

        private int DistanceToCenter(
            RecyclerView.LayoutManager layoutManager,
            View targetView,
            OrientationHelper horizontalHelper)
        {
            int childCenter = (int)(targetView.GetX() + targetView.Width / 2);
            int containerCenter = horizontalHelper.End / 2;
            return childCenter - containerCenter;
        }

        public void ScrollTo(int viewPagerCurrentItem)
        {
            var dest =
                _recyclerView.FindViewHolderForAdapterPosition(viewPagerCurrentItem) ??
                _recyclerView.FindViewHolderForLayoutPosition(0);

            var distances = CalculateDistanceToFinalSnap(
                _recyclerView.GetLayoutManager(),dest.ItemView);
            _recyclerView.SmoothScrollBy(distances[0], distances[1]);
        }
    }
}