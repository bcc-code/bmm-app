using Android.Runtime;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Listeners;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.BrowseFragment")]
    public class BrowseFragment : BaseFragment<BrowseViewModel>
    {
        private BrowseRecyclerAdapter _browseRecyclerAdapter;
        private MvxSwipeRefreshLayout _swipeRefreshLayout;
        private AppBarLayout _appBarLayout;

        private BrowseRecyclerAdapter BrowseRecyclerAdapter =>
            _browseRecyclerAdapter ??= new BrowseRecyclerAdapter((IMvxAndroidBindingContext) BindingContext);

        private MvxSwipeRefreshLayout SwipeRefreshLayout
            => _swipeRefreshLayout ??= View.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);

        private AppBarLayout AppBarLayout
            => _appBarLayout ??= View.FindViewById<AppBarLayout>(Resource.Id.appbar);

        protected override void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            base.InitRecyclerView(recyclerView);
            recyclerView.SetItemViewCacheSize(20);
            recyclerView!.Adapter = BrowseRecyclerAdapter;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();

            if (AppBarLayout != null && SwipeRefreshLayout != null)
                AppBarLayout.OffsetChanged += AppBarOnOffsetChanged;

            RecyclerView.AddOnScrollListener(new ImageServiceRecyclerViewScrollListener());
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();

            if (AppBarLayout != null)
                AppBarLayout.OffsetChanged -= AppBarOnOffsetChanged;

            RecyclerView.ClearOnScrollListeners();
        }

        private void AppBarOnOffsetChanged(object sender, AppBarLayout.OffsetChangedEventArgs e)
        {
            SwipeRefreshLayout.Enabled = e.VerticalOffset == 0;
        }

        protected override int FragmentId => Resource.Layout.fragment_browse;
    }
}