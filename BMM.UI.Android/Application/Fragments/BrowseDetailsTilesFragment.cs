using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.ItemDecorators;
using FFImageLoading.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.BrowseDetailsTilesFragment")]
    public class BrowseDetailsTilesFragment : BaseFragment<BrowseDetailsTilesViewModel>
    {
        private const int ItemSpacing = 12;
        private const int ItemsPerRow = 2;

        protected override int FragmentId => Resource.Layout.fragment_browse_details_tiles;

        protected override bool HasCustomTitle => true;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            Title = ViewModel.Title;
            return view;
        }

        protected override void Bind()
        {
            base.Bind();

            var set = this.CreateBindingSet<BrowseDetailsTilesFragment, BrowseDetailsTilesViewModel>();

            set.Bind(this)
                .For(v => v.Title)
                .To(vm => vm.Title);

            set.Apply();
        }

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