using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.FollowedPodcastsFragment")]
    public class FollowedPodcastsFragment : BaseFragment<FollowedPodcastsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_followedpodcasts;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            InitRecyclerView(view);
            SetStatusBarColor(ColorOfUppermostFragment());

            return view;
        }

        private void InitRecyclerView(View view)
        {
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.my_recycler_view);
            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;
                var layoutManager = new GridLayoutManager(ParentActivity, 2);
                recyclerView.SetLayoutManager(layoutManager);
            }
        }
    }
}