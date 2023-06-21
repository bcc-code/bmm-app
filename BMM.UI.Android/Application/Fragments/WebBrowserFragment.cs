using Android.Runtime;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = true, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.WebBrowserFragment")]
    public class WebBrowserFragment : WebBrowserBaseFragment<WebBrowserViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_web_browser;
        protected override int WebViewContainerId => Resource.Id.WebViewContainer;
        protected override bool ShouldWebViewInterceptTouches => false;
    }
}