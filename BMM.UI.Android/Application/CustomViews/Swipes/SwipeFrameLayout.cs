using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace BMM.UI.Droid.Application.CustomViews.Swipes
{
    public class SwipeFrameLayout : FrameLayout
    {
        private const float MinDeltaForSwipe = 10;

        private float? _touchPointX;

        protected SwipeFrameLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SwipeFrameLayout(Context context) : base(context)
        {
        }

        public SwipeFrameLayout(Context context, IAttributeSet? attrs) : base(context, attrs)
        {
        }

        public SwipeFrameLayout(Context context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public SwipeFrameLayout(Context context, IAttributeSet? attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent? ev)
        {
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    SetTouchPointX(ev);
                    break;
                case MotionEventActions.Move:
                    float delta = GetDeltaX(ev, _touchPointX);
                    if (Math.Abs(delta) > MinDeltaForSwipe)
                        return true;

                    break;
            }
            return base.OnInterceptTouchEvent(ev);
        }

        private float GetDeltaX(MotionEvent? ev, float? touchPointX)
            => touchPointX is null
                ? 0
                : ev.GetX() - touchPointX.Value;

        private void SetTouchPointX(MotionEvent? ev)
        {
            _touchPointX = ev.GetX();
        }
    }
}