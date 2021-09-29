using System;
using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class FraKaareTeaserViewHolder : MvxRecyclerViewHolder
    {
        public FraKaareTeaserViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected FraKaareTeaserViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        private void Bind()
        {
            var constraintLayout = ItemView.FindViewById<ConstraintLayout>(Resource.Id.FraKaareConstraintLayout);
            constraintLayout!.ClipToOutline = true;
        }
    }
}