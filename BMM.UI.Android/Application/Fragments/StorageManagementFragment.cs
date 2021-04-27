using Android.Runtime;
using BMM.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.StorageManagementFragment")]
    public class StorageManagementFragment : BaseFragment<StorageManagementViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_storage_management;
    }
}