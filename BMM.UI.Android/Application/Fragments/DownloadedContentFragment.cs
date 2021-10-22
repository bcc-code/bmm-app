using Android.Runtime;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.DownloadedContentFragment")]
    public class DownloadedContentFragment : SwipeToRefreshFragment<DownloadedContentViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_downloadedcontent;
    }
}