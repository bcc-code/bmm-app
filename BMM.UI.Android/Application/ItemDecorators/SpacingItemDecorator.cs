using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ItemDecorators.Enums;

namespace BMM.UI.Droid.Application.ItemDecorators
{
    public class SpacingItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _sideSpacing;
        private readonly int _spacing;

        public SpacingItemDecoration(
            int spacing,
            int sideSpacing = 0)
        {
            _sideSpacing = sideSpacing;
            _spacing = spacing;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);

            int childPosition = parent.GetChildAdapterPosition(view);
            int allElementsCount = parent.GetAdapter().ItemCount;

            if (childPosition == 0)
            {
                outRect.Left = _sideSpacing;
            }
            else if (childPosition == allElementsCount - 1)
            {
                outRect.Right = _sideSpacing;
                outRect.Left = _spacing;
            }
            else
                outRect.Left = _spacing;
        }
    }
}
