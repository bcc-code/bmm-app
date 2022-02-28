using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace BMM.UI.Droid.Application.CustomViews
{
    [Register("bmm.ui.droid.application.customViews.ShadowLayout")]
    public class ShadowLayout : FrameLayout
    {
        private static int _maxAlpha = 255;
        private static float _minRadius = 0.1f;

        private readonly Rect _bounds = new Rect();
        private readonly Canvas _canvas = new Canvas();

        private readonly Paint _paint = new Paint(PaintFlags.AntiAlias)
        {
            Dither = true,
            FilterBitmap = true
        };

        private Bitmap _bitmap;

        private bool _invalidateShadow = true;

        private bool _isShadowed;

        private Color _shadowColor;
        private int _shadowAlpha;
        private float _shadowRadius;
        private float _shadowDistance;
        private float _shadowAngle;
        private float _shadowDx;
        private float _shadowDy;

        public ShadowLayout(Context context) : this(context, null)
        {
        }

        public ShadowLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public ShadowLayout(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        {
            SetWillNotDraw(false);
            SetLayerType(LayerType.Hardware, _paint);
            
            _shadowAngle = 360;
            SetIsShadowed(true);
        }

        public float TopPaddingMultiplier { get; set; } = 1;

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            if (_bitmap == null)
                return;
            
            _bitmap.Recycle();
            _bitmap = null;
        }

        public void SetIsShadowed(bool isShadowed)
        {
            this._isShadowed = isShadowed;
            PostInvalidate();
        }

        public void SetShadowRadius(float shadowRadius)
        {
            _shadowRadius = Math.Max(_minRadius, shadowRadius);
            if (IsInEditMode)
                return;

            _paint.SetMaskFilter(new BlurMaskFilter(_shadowRadius, BlurMaskFilter.Blur.Normal));
            ResetShadow();
        }

        public void SetShadowColor(Color shadowColor)
        {
            _shadowColor = shadowColor;
            _shadowAlpha = shadowColor.A;
            ResetShadow();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            _bounds.Set(0,
                0,
                MeasuredWidth,
                MeasuredHeight);
        }

        private void ResetShadow()
        {
            _shadowDx = (float)(_shadowDistance * Math.Cos(_shadowAngle / (180 * Math.Pi)));
            _shadowDy = (float)(_shadowDistance * Math.Sin(_shadowAngle / (180 * Math.Pi)));
            
            int padding = (int)(_shadowDistance + _shadowRadius);
            SetPadding(padding,
                (int)(padding * TopPaddingMultiplier),
                padding,
                padding);
            RequestLayout();
        }

        private Color AdjustShadowAlpha(bool adjust)
        {
            return Color.Argb(
                adjust
                    ? _maxAlpha
                    : _shadowAlpha,
                _shadowColor.R,
                _shadowColor.G,
                _shadowColor.B
            );
        }

        public override void RequestLayout()
        {
            _invalidateShadow = true;
            base.RequestLayout();
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            if (_isShadowed)
            {
                if (_invalidateShadow)
                {
                    if (_bounds.Width() != 0
                        && _bounds.Height() != 0)
                    {
                        _bitmap = Bitmap.CreateBitmap(_bounds.Width(), _bounds.Height(), Bitmap.Config.Argb8888!);
                        _canvas.SetBitmap(_bitmap);
                        _invalidateShadow = false;
                        base.DispatchDraw(_canvas);
                        var extractedAlphaBitmap = _bitmap.ExtractAlpha();
                        _canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear!);
                        _paint.Color = AdjustShadowAlpha(false);
                        _canvas.DrawBitmap(extractedAlphaBitmap,
                            _shadowDx,
                            _shadowDy,
                            _paint);
                        extractedAlphaBitmap.Recycle();
                    }
                    else
                    {
                        _bitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Rgb565!);
                    }

                }

                _paint.Color = AdjustShadowAlpha(true);

                if (_canvas != null && _bitmap != null && !_bitmap.IsRecycled)
                    canvas.DrawBitmap(_bitmap, 0, 0, _paint);
            }

            base.DispatchDraw(canvas);
        }
    }
}