using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class InfoMessageViewHolder : MvxRecyclerViewHolder
    {
        public InfoMessageViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected InfoMessageViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        private void Bind()
        {
            var constraintLayout = ItemView.FindViewById<LinearLayout>(Resource.Id.InfoMessageLayout);
            constraintLayout!.ClipToOutline = true;
        }
    }
}