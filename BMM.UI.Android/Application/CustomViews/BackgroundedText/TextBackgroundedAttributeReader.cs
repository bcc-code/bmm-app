using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Util;
using AndroidX.Core.Content.Resources;

namespace BMM.UI.Droid.Application.CustomViews.BackgroundedText
{
    public class TextBackgroundedAttributeReader
    {
        public int HorizontalPadding { get; }
        public int VerticalPadding { get; }
        public Drawable Drawable { get; }

        public TextBackgroundedAttributeReader(Context context, IAttributeSet attrs)
        {
            TypedArray typedArray = context.ObtainStyledAttributes(attrs,
                Resource.Styleable.BmmTextView, 0, 0);
            
            try
            {
                HorizontalPadding = typedArray.GetDimensionPixelSize(
                    Resource.Styleable.BmmTextView_roundedTextHorizontalPadding, 0);
                VerticalPadding = typedArray.GetDimensionPixelSize(
                    Resource.Styleable.BmmTextView_roundedTextVerticalPadding, 0);
                Drawable = ResourcesCompat.GetDrawable(
                    context.Resources, typedArray.GetResourceId(
                        Resource.Styleable.BmmTextView_roundedTextDrawable, -1), null)
                    ?? throw new ArgumentException("Drawable not found.");
            }
            finally
            {
                typedArray.Recycle();
            }
        }
    }
}