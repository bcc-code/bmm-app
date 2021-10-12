using System;
using Android.Content;
using Android.Runtime;
using Android.Sax;
using Android.Views;
using BMM.UI.Droid.Application.Extensions;
using FFImageLoading.Cross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class CoverWithTitleViewHolder : MvxRecyclerViewHolder
    {
        private readonly int _elementWidth;
        private MvxCachedImageView _coverImageView;
        private const int Unspecified = -1;

        private MvxCachedImageView CoverImageView =>
            _coverImageView ??= ItemView.FindViewById<MvxCachedImageView>(Resource.Id.CoverImageView);

        public CoverWithTitleViewHolder(View itemView, IMvxAndroidBindingContext context, int elementWidth = Unspecified) : base(itemView, context)
        {
            _elementWidth = elementWidth;
            this.DelayBind(Bind);
        }

        private void Bind()
        {
            var imageView = ItemView.FindViewById<MvxCachedImageView>(Resource.Id.CoverImageView);
            imageView!.ClipToOutline = true;
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (_elementWidth == Unspecified)
                return;

            CoverImageView.UpdateHeight(_elementWidth);
        }
    }
}