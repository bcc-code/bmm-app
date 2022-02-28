using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = true, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.ChangeTrackLanguageFragment")]
    public class ChangeTrackLanguageFragment : BaseDialogFragment<ChangeTrackLanguageViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_change_track_language;
    }
}