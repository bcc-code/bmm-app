using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using Com.Airbnb.Lottie;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.AskQuestionConfirmationFragment")]
    public class AskQuestionConfirmationFragment : BaseDialogFragment<AskQuestionConfirmationViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_ask_question_confirmation;
    }
}