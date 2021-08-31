using System;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
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
            var coversCarouselCollectionRecyclerView = ItemView.FindViewById<MvxRecyclerView>(Resource.Id.CoversCarouselCollectionRecyclerView);
            var layoutManager = new LinearLayoutManager(ItemView.Context, LinearLayoutManager.Horizontal, false);

            var spacingItemDecoration = new SpacingItemDecoration(
                ItemSpacing.DpToPixels(),
                SideSpacing.DpToPixels());

            coversCarouselCollectionRecyclerView!.AddItemDecoration(spacingItemDecoration);
            coversCarouselCollectionRecyclerView.SetLayoutManager(layoutManager);
            coversCarouselCollectionRecyclerView.Adapter = new CoversCarouselRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }
    }
}