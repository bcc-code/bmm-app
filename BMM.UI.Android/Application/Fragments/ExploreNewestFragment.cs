using Android.Runtime;
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
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var swipeToRefresh = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);
            var appBar = Activity.FindViewById<AppBarLayout>(Resource.Id.appbar);
            if (appBar != null && swipeToRefresh != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            //// ToDo: Since the ImageViews are null we have to find another solution to changing the layout for smaller devices
            //var radioBackgroundLiveUpcoming = view.FindViewById<ImageView>(Resource.Id.radioBackgroundLiveUpcoming);
            //var radioBackgroundLiveBroadcasting = view.FindViewById<ImageView>(Resource.Id.radioBackgroundLiveBroadcasting);
            //if (Resources.DisplayMetrics.WidthPixels <= 480)
            //{
            //    radioBackgroundLiveUpcoming.SetScaleType(ImageView.ScaleType.CenterCrop);
            //    radioBackgroundLiveUpcoming.SetPadding(0, 0, 50, 0);

            //    radioBackgroundLiveBroadcasting.SetScaleType(ImageView.ScaleType.CenterCrop);
            //    radioBackgroundLiveBroadcasting.SetPadding(0, 0, 50, 0);
            //}
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.my_recycler_view);
            recyclerView.Adapter = CreateAdapter();

            return view;
        }

        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new PodcastContextHeaderRecyclerAdapter((IMvxAndroidBindingContext) BindingContext, ViewModel);
        }

        protected override int FragmentId => Resource.Layout.fragment_explore_with_appbar;
    }
}