using System;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace BMM.UI.Droid.Application.ItemDecorators
{
    public class SpacingItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _sideSpacing;
        private readonly int _itemsPerLine;
        private readonly int _horizontalSpacing;
        private readonly int _verticalSpacing;

        public SpacingItemDecoration(
            int horizontalSpacing = 0,
            int verticalSpacing = 0,
            int sideSpacing = 0,
            int itemsPerLine = 1)
        {
            _horizontalSpacing = horizontalSpacing;
            _verticalSpacing = verticalSpacing;
            _sideSpacing = sideSpacing;
            _itemsPerLine = itemsPerLine;
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
                SetLeftHorizontalSpacing(outRect, childPosition);
            }
            else
                SetLeftHorizontalSpacing(outRect, childPosition);

            if (IsInFirstRow(childPosition))
                outRect.Top = _verticalSpacing;

            outRect.Bottom = _verticalSpacing;
        }

        private void SetLeftHorizontalSpacing(Rect outRect, int childPosition)
        {
            if (!IsMultiColumn || !IsFirstInLine(childPosition))
                outRect.Left = _horizontalSpacing;
        }

        private bool IsMultiColumn => _itemsPerLine > 1;

        private bool IsInFirstRow(int childPosition)
            => childPosition + 1 <= _itemsPerLine;

        private bool IsFirstInLine(int childPosition)
            => childPosition % _itemsPerLine == 0;
    }
}
