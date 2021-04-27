using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;

namespace BMM.UI.Droid.Application.Fragments
{
    [Register("bmm.ui.droid.application.fragments.ExplorePopularFragment")]
    public class ExplorePopularFragment : BaseFragment<ExplorePopularViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var swipeToRefresh = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);
            var appBar = Activity.FindViewById<AppBarLayout>(Resource.Id.appbar);
            if (appBar != null && swipeToRefresh != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            return view;
        }

        protected override string Title => string.Empty;

        protected override int FragmentId => Resource.Layout.fragment_explore_page;
    }
}