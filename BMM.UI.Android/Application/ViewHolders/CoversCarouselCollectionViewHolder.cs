using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.ItemDecorators;
using FFImageLoading.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class CoversCarouselCollectionViewHolder : MvxRecyclerViewHolder
    {
        private MvxRecyclerView _coversCarouselCollectionRecyclerView;
        private IParcelable _state;
        private LinearLayoutManager _layoutManager;
        private const int ItemSpacing = 12;
        private const int SideSpacing = 16;

        public CoversCarouselCollectionViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected CoversCarouselCollectionViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        private void Bind()
        {
            _coversCarouselCollectionRecyclerView = ItemView.FindViewById<MvxRecyclerView>(Resource.Id.CoversCarouselCollectionRecyclerView);
            _layoutManager = new LinearLayoutManager(ItemView.Context, LinearLayoutManager.Horizontal, false);

            var spacingItemDecoration = new SpacingItemDecoration(
                ItemSpacing.DpToPixels(),
                SideSpacing.DpToPixels());

            var adapter = new CoversCarouselRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);

            _coversCarouselCollectionRecyclerView!.AddItemDecoration(spacingItemDecoration);
            _coversCarouselCollectionRecyclerView.HasFixedSize = true;
            _coversCarouselCollectionRecyclerView.SetLayoutManager(_layoutManager);
            _coversCarouselCollectionRecyclerView.Adapter = adapter;
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            _layoutManager.OnRestoreInstanceState(_state);
        }

        public override void OnDetachedFromWindow()
        {
            _state = _layoutManager.OnSaveInstanceState();
            base.OnDetachedFromWindow();
        }
    }
}