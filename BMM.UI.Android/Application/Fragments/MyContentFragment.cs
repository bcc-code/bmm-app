using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.MyContent;
using BMM.UI.Droid.Application.Fragments.Base;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.MyContentFragment")]
    public class MyContentFragment : SwipeToRefreshFragment<MyContentViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_mycontent;
    }
}