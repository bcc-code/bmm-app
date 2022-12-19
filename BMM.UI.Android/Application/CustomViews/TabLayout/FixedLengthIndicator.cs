using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using BMM.UI.Droid.Application.Extensions;

namespace BMM.UI.Droid.Application.CustomViews.TabLayout
{
    [Register("bmm.ui.droid.application.customViews.FixedLengthIndicator")]
    public class FixedLengthIndicator : View
    {
        public const int AnimationDuration = 300;

        private Paint _underLinePaint;
        private Paint _indicatorPaint;
        private int _lastPosition;
        private ValueAnimator _animator;
        private int _itemWidth;

        protected FixedLengthIndicator(IntPtr javaReference, JniHandleOwnership transfer) : base(
            javaReference,
            transfer)
        {
        }

        public FixedLengthIndicator(Context context) : base(context)
        {
        }

        public FixedLengthIndicator(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetUp();
        }

        public FixedLengthIndicator(Context context, IAttributeSet attrs, int defStyleAttr) : base(
            context,
            attrs,
            defStyleAttr)
        {
            SetUp();
        }

        public FixedLengthIndicator(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(
            context,
            attrs,
            defStyleAttr,
            defStyleRes)
        {
            SetUp();
        }

        public Color IndicatorColor { get; set; }
        public Color UnderlineColor { get; set; }
        public int LastPosition => _lastPosition;
        public float MinAnimationDistance => 20 * DpFactor;
        public float UnderlineHeight { get; set; } = 3;
        public float UnderlineHeightDp => UnderlineHeight * DpFactor;
        public float IndicatorWidth { get; set; } = 20;
        public float IndicatorWidthDp => IndicatorWidth * DpFactor;
        public float ShadowRadius { get; set; } = 4;
        public float DpFactor => Resources.DisplayMetrics.Density;
        public float ItemWidth => _itemWidth == 0
            ? IndicatorWidthDp
            : _itemWidth;

        private void SetUp()
        {
            _indicatorPaint = new Paint(PaintFlags.AntiAlias);
            _underLinePaint = new Paint(PaintFlags.AntiAlias);
            _lastPosition = Resources.DisplayMetrics.WidthPixels / 2;
            IndicatorColor = Context.GetColorFromResource(Resource.Color.label_primary_color);
            SetWillNotDraw(false);
        }

        public void MoveTo(int x, int width)
        {
            _itemWidth = width;
            int delta = Math.Abs(_lastPosition - x);

            if (delta < MinAnimationDistance && _animator == null)
            {
                _lastPosition = x;
                return;
            }

            CancelAnimatorIfNeeded();
            CreateAnimator(x);
        }

        private void CancelAnimatorIfNeeded()
        {
            if (_animator?.IsRunning ?? false)
            {
                _animator.Cancel();
                _animator = null;
            }
        }

        private void CreateAnimator(int toPosition)
        {
            _animator = ValueAnimator.OfInt(_lastPosition, toPosition);
            _animator.SetDuration(AnimationDuration);
            _animator.Update += AnimatorOnUpdate;
            _animator.AnimationEnd += AnimatorOnAnimationEnd;
            _animator.Start();
        }

        private void AnimatorOnAnimationEnd(object sender, EventArgs e)
        {
            _animator = null;
        }

        private void AnimatorOnUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e)
        {
            _lastPosition = (int)e.Animation.AnimatedValue;
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            _indicatorPaint.Color = IndicatorColor;
            _underLinePaint.Color = UnderlineColor;

            //just to be sure it's not updated during draw
            int centerPosX = _lastPosition;

            DrawUnderline(canvas, _underLinePaint);
            DrawIndicator(canvas, centerPosX);

            base.OnDraw(canvas);
        }

        private void DrawIndicator(Canvas canvas, int centerPosX) => canvas.DrawRect(
            centerPosX - (ItemWidth / 2),
            0,
            centerPosX + (ItemWidth / 2),
            canvas.Height,
            _indicatorPaint);

        private void DrawUnderline(Canvas canvas, Paint underLinePaint) => canvas.DrawRect(
            0,
            (canvas.Height - UnderlineHeightDp) / 2,
            canvas.Width,
            (canvas.Height + UnderlineHeightDp) / 2,
            underLinePaint);

        public void SetTo(int newPosition, int width)
        {
            _itemWidth = width;
            CancelAnimatorIfNeeded();

            _lastPosition = newPosition;
            Invalidate();
        }
    }
}