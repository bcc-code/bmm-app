using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace BMM.UI.Droid.Application.ItemDecorators;

public class BottomOffsetDecoration : RecyclerView.ItemDecoration
{
    private readonly int _offset;

    public BottomOffsetDecoration(int offset)
    {
        _offset = offset;
    }

    public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
    {
        base.GetItemOffsets(outRect, view, parent, state);

        if (parent.GetChildAdapterPosition(view) == parent.GetAdapter()!.ItemCount - 1)
            outRect.Bottom = _offset;
        else
            outRect.Bottom = 0;
    }
}