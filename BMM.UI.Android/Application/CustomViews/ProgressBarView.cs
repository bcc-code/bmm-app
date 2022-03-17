using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using BMM.UI.Droid.Application.Extensions;
using FFImageLoading.Extensions;

namespace BMM.UI.Droid.Application.CustomViews
{
    [Register("bmm.ui.droid.application.customViews.ProgressBarView")]
    public class ProgressBarView : View
    {
        private const float PercentageFactor = 100f;
        private int ProgressBarHeight => 2.DpToPixels();
        private Paint _backgroundPaint;
        private Paint _foregroundPaint;
        private int? _percentage;

        public ProgressBarView(Context? context, IAttributeSet? attrs)
            : base(context, attrs)
        {
            Initialize(attrs);
        }

        private void Initialize(
            IAttributeSet? attrs,
            int defStyleAttr = 0,
            int defStyleRes = 0)
        {
            _backgroundPaint ??= new Paint(PaintFlags.AntiAlias);
            _backgroundPaint.SetStyle(Paint.Style.Fill);

            _foregroundPaint ??= new Paint(PaintFlags.AntiAlias);
            _foregroundPaint.SetStyle(Paint.Style.Fill);

            ApplyStyle();
        }

        public int? Percentage
        {
            get => _percentage;
            set
            {
                if (_percentage == value || !value.HasValue)
                    return;

                _percentage = value;
                SetColors();
            }
        }

        public void SetColors()
        {
            _backgroundPaint.Color = Context.GetColorFromResource(Resource.Color.on_color_five_color);
            _foregroundPaint.Color = Context.GetColorFromResource(Resource.Color.on_color_one_color);
            Invalidate();
        }

        public void ApplyStyle(bool animate = false)
        {
            SetColors();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            canvas.DrawRect(0, 0, canvas.Width, ProgressBarHeight, _backgroundPaint);
            canvas.DrawRect(0, 0, CalculateRightForForegroundPaint(canvas), ProgressBarHeight, _foregroundPaint);
        }

        private float CalculateRightForForegroundPaint(Canvas canvas)
            => canvas.Width * (Percentage.Value / PercentageFactor);

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetMeasuredDimension(MeasuredWidth, ProgressBarHeight);
        }
    }
}