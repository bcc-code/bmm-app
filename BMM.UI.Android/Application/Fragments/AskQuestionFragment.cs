using Android.Runtime;
using Android.Views.InputMethods;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.AskQuestionFragment")]
    public class AskQuestionFragment : BaseDialogFragment<AskQuestionViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_ask_question;

        public override void OnResume()
        {
            base.OnResume();
            View!.Post(() =>
            {
                var questionEditText = View.FindViewById<EditText>(Resource.Id.QuestionEditText);
                questionEditText.RequestFocus();
                var imm = (InputMethodManager)Context!.GetSystemService(Android.Content.Context.InputMethodService);
                imm!.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            });
        }
    }
}