using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace BMM.UI.Droid.Application.CustomViews;

[Register("bmm.ui.droid.application.customViews.NonTouchableHorizontalScrollView")]
public class NonTouchableHorizontalScrollView : HorizontalScrollView
{
    protected NonTouchableHorizontalScrollView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public NonTouchableHorizontalScrollView(Context context) : base(context)
    {
    }

    public NonTouchableHorizontalScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
    }

    public NonTouchableHorizontalScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
    }

    public NonTouchableHorizontalScrollView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr,
        defStyleRes)
    {
    }

    public override bool OnTouchEvent(MotionEvent e)
    {
        return false;
    }

    public override bool OnInterceptTouchEvent(MotionEvent ev)
    {
        return false;
    }
}