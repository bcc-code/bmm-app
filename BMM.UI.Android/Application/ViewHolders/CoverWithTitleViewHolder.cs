using System;
using Android.Runtime;
using Android.Views;
using FFImageLoading.Cross;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class CoverWithTitleViewHolder : MvxRecyclerViewHolder
    {
        public CoverWithTitleViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
        }

        protected CoverWithTitleViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            var imageView = ItemView.FindViewById<MvxCachedImageView>(Resource.Id.CoverImageView);
            imageView!.ClipToOutline = true;
        }
    }
}