using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.PodcastsFragment")]
    public class PodcastsFragment : BaseCoversTileCollectionFragment<PodcastsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_podcasts;
    }
}