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
    public class TilesCollectionViewHolder : MvxRecyclerViewHolder
    {
        private MvxRecyclerView _tilesCollectionRecyclerView;
        private IParcelable _state;
        private LinearLayoutManager _layoutManager;
        private const int ItemSpacing = 12;
        private const int SideSpacing = 16;

        public TilesCollectionViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected TilesCollectionViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        private void Bind()
        {
            _tilesCollectionRecyclerView = ItemView.FindViewById<MvxRecyclerView>(Resource.Id.TilesCollectionRecyclerView);
            _tilesCollectionRecyclerView!.NestedScrollingEnabled = false;

            _layoutManager = new LinearLayoutManager(ItemView.Context, LinearLayoutManager.Horizontal, false)
            {
                RecycleChildrenOnDetach = true
            };

            var spacingItemDecoration = new SpacingItemDecoration(ItemSpacing.DpToPixels(), sideSpacing: SideSpacing.DpToPixels());
            var adapter = new TilesCollectionRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
            
            _tilesCollectionRecyclerView!.AddItemDecoration(spacingItemDecoration);
            _tilesCollectionRecyclerView.HasFixedSize = true;
            _tilesCollectionRecyclerView.SetLayoutManager(_layoutManager);
            _tilesCollectionRecyclerView.Adapter = adapter;
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