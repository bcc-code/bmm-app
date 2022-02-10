using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using FFImageLoading.Cross;

namespace BMM.UI.Droid.Application.CustomViews
{
    [Register("bmm.ui.droid.application.customViews.BmmCachedImageView")]
    public class BmmCachedImageView : MvxCachedImageView
    {
        public BmmCachedImageView(Context context) : base(context)
        {
        }

        public BmmCachedImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public BmmCachedImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public event EventHandler ImageChanged;
        
        public override void SetImageBitmap(Bitmap? bm)
        {
            base.SetImageBitmap(bm);
            ImageChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void SetImageDrawable(Drawable? drawable)
        {
            base.SetImageDrawable(drawable);
            ImageChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void SetImageResource(int resId)
        {
            base.SetImageResource(resId);
            ImageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}