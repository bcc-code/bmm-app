using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace BMM.UI.Droid.Application.Decorators
{
    public class HorizontalSpacingItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _spacing;
        private readonly int _additionalHorizontalMargin;

        public HorizontalSpacingItemDecoration(int spacing, int additionalHorizontalMargin)
        {
            _spacing = spacing;
            _additionalHorizontalMargin = additionalHorizontalMargin;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Left = _spacing;
            outRect.Right = _spacing;

            if (IsFirstItem(parent, view))
                outRect.Left += _additionalHorizontalMargin;

            if (IsLastItem(parent, view))
                outRect.Right += _additionalHorizontalMargin;
        }

        private bool IsLastItem(RecyclerView parent, View view)
            => parent.GetChildAdapterPosition(view) == parent.GetAdapter().ItemCount - 1;

        private bool IsFirstItem(RecyclerView parent, View view)
            => parent.GetChildAdapterPosition(view) == 0;
    }
}