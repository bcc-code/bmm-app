using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.ItemDecorators;
using FFImageLoading.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.CuratedPlaylistsFragment")]
    public class CuratedPlaylistsFragment : BaseFragment<CuratedPlaylistsViewModel>
    {
        private const int ItemSpacing = 12;
        private const int ItemsPerRow = 2;

        protected override int FragmentId => Resource.Layout.fragment_browse_details_tiles;

        protected override void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            if (recyclerView == null)
                return;

            var spacingItemDecoration = new SpacingItemDecoration(
                ItemSpacing.DpToPixels(),
                ItemSpacing.DpToPixels(),
                itemsPerLine: ItemsPerRow);

            recyclerView!.AddItemDecoration(spacingItemDecoration);
            recyclerView.HasFixedSize = true;

            var layoutManager = new GridLayoutManager(ParentActivity, ItemsPerRow);
            recyclerView.SetLayoutManager(layoutManager);

            recyclerView.Adapter = new CoversCarouselRecyclerAdapter(
                (IMvxAndroidBindingContext)BindingContext,
                recyclerView,
                ItemsPerRow,
                ItemSpacing);
        }
    }
}