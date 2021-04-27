using Android.Runtime;
using BMM.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.TrackCollectionsAddToFragment")]
    public class TrackCollectionsAddToFragment : BaseFragment<TrackCollectionsAddToViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_trackcollections_addto;
    }
}