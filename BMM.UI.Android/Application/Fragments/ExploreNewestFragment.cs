using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
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
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var swipeToRefresh = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);
            var appBar = Activity.FindViewById<AppBarLayout>(Resource.Id.appbar);
            if (appBar != null && swipeToRefresh != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.my_recycler_view);
            recyclerView.Adapter = CreateAdapter();

            return view;
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

        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new PodcastContextHeaderRecyclerAdapter((IMvxAndroidBindingContext) BindingContext, ViewModel);
        }

        protected override int FragmentId => Resource.Layout.fragment_explore_with_appbar;
    }
}