using Android.OS;
using Android.Runtime;
using Android.Views;
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
    [Register("bmm.ui.droid.application.fragments.ExploreNewestFragment")]
    public class ExploreNewestFragment : BaseFragment<ExploreNewestViewModel>
    {
        private PodcastContextHeaderRecyclerAdapter _podcastContextHeaderRecyclerAdapter;
        private MvxSwipeRefreshLayout _swipeRefreshLayout;
        private AppBarLayout _appBarLayout;

        private PodcastContextHeaderRecyclerAdapter PodcastContextHeaderRecyclerAdapter =>
            _podcastContextHeaderRecyclerAdapter ??= new PodcastContextHeaderRecyclerAdapter((IMvxAndroidBindingContext) BindingContext, ViewModel);

        private MvxSwipeRefreshLayout SwipeRefreshLayout
            => _swipeRefreshLayout ??= View.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);

        private AppBarLayout AppBarLayout
            => _appBarLayout ??= View.FindViewById<AppBarLayout>(Resource.Id.appbar);

        protected override void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            base.InitRecyclerView(recyclerView);
            recyclerView.SetItemViewCacheSize(20);
            recyclerView!.Adapter = PodcastContextHeaderRecyclerAdapter;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.playback_history, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.playback_history)
                ViewModel.NavigateToViewModelCommand.Execute(typeof(PlaybackHistoryViewModel));

            return base.OnOptionsItemSelected(item);
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

        protected override int FragmentId => Resource.Layout.fragment_explore;
    }
}