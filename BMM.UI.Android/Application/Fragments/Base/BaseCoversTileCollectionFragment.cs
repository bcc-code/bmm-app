using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels.Base;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.ItemDecorators;
using FFImageLoading.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Fragments.Base
{
    public abstract class BaseCoversTileCollectionFragment<TViewModel> : BaseFragment<TViewModel> where TViewModel : BaseViewModel, IMvxViewModel
    {
        private const int ItemSpacing = 12;
        private const int ItemsPerRow = 2;

        protected override void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            if (recyclerView == null)
                return;

            int itemSpacing = ItemSpacing.DpToPixels();

            var spacingItemDecoration = new SpacingItemDecoration(
                itemSpacing / 2,
                itemSpacing,
                itemsPerLine: ItemsPerRow);

            recyclerView!.AddItemDecoration(spacingItemDecoration);
            recyclerView.HasFixedSize = true;

            var layoutManager = new GridLayoutManager(ParentActivity, ItemsPerRow);
            recyclerView.SetLayoutManager(layoutManager);

            recyclerView.Adapter = new CoversCarouselRecyclerAdapter(
                (IMvxAndroidBindingContext)BindingContext,
                recyclerView,
                ItemsPerRow,
                itemSpacing);
        }
    }
}