using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using BMM.UI.Droid.Application.Adapters;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.MyTracksFragment")]
    public class MyTracksFragment : BaseFragment<MyTracksViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var swipeToRefresh = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);
            var appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar);

            if (appBar != null && swipeToRefresh != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            InitRecyclerView(view);

            return view;
        }

        private void InitRecyclerView(View view)
        {
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.trackcollection_recycler_view);
            if (recyclerView != null)
            {
                recyclerView.Adapter = new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
                recyclerView.HasFixedSize = true;

                var layoutManager = new LinearLayoutManager(ParentActivity);
                recyclerView.SetLayoutManager(layoutManager);
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_mytracks;
    }
}