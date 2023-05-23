using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.TrackInfoFragment")]
    public class TrackInfoFragment : BaseFragment<TrackInfoViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_trackinfo;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            Title = ViewModel.Track.Meta.Title;
            base.OnViewCreated(view, savedInstanceState);
        }
    }
}