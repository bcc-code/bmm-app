using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using Com.Airbnb.Lottie;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.SuggestEditFragment")]
    public class SuggestEditFragment : BaseDialogFragment<SuggestEditViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_suggest_edit;
    }
}