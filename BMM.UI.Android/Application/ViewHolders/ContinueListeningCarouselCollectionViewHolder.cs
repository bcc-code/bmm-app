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
    public class ContinueListeningCarouselCollectionViewHolder : MvxRecyclerViewHolder
    {
        private MvxRecyclerView _continueListeningCarouselCollectionRecyclerView;
        private IParcelable _state;
        private LinearLayoutManager _layoutManager;
        private const int ItemSpacing = 12;
        private const int SideSpacing = 16;

        public ContinueListeningCarouselCollectionViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected ContinueListeningCarouselCollectionViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        private void Bind()
        {
            _continueListeningCarouselCollectionRecyclerView = ItemView.FindViewById<MvxRecyclerView>(Resource.Id.ContinueListeningCarouselCollectionRecyclerView);
            _continueListeningCarouselCollectionRecyclerView!.NestedScrollingEnabled = false;

            _layoutManager = new LinearLayoutManager(ItemView.Context, LinearLayoutManager.Horizontal, false)
            {
                RecycleChildrenOnDetach = true
            };

            var spacingItemDecoration = new SpacingItemDecoration(ItemSpacing.DpToPixels(), sideSpacing: SideSpacing.DpToPixels());
            var adapter = new ContinueAudiobookCarouselRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
            
            _continueListeningCarouselCollectionRecyclerView!.AddItemDecoration(spacingItemDecoration);
            _continueListeningCarouselCollectionRecyclerView.HasFixedSize = true;
            _continueListeningCarouselCollectionRecyclerView.SetLayoutManager(_layoutManager);
            _continueListeningCarouselCollectionRecyclerView.Adapter = adapter;
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