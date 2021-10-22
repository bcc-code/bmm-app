using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.ExploreRecentSpeechesFragment")]
    public class ExploreRecentSpeechesFragment : SwipeToRefreshFragment<ExploreRecentSpeechesViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_explore;
    }
}