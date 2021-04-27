using Android.Views.InputMethods;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Activities
{
    public class BaseFragmentActivity<TViewModel> : MvxActivity<TViewModel>
        where TViewModel : class, IMvxViewModel
    {
        private InputMethodManager InputMethodManager => (InputMethodManager) GetSystemService(InputMethodService);

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount <= 1)
            {
                FinishAffinity();
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }
}
