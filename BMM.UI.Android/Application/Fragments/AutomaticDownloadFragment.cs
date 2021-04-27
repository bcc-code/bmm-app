using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.AutomaticDownloadFragment")]
    public class AutomaticDownloadFragment : BaseFragment<AutomaticDownloadViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_automatic_download;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            SetStatusBarColor(ColorOfUppermostFragment());

            return view;
        }
    }
}