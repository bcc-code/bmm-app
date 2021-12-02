using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.FollowedPodcastsFragment")]
    public class FollowedPodcastsFragment : BaseCoversTileCollectionFragment<FollowedPodcastsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_followedpodcasts;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            SetStatusBarColor(ColorOfUppermostFragment());

            return view;
        }
    }
}