using Android.Views.InputMethods;
using BMM.Core.Implementations.Storage;
using BMM.UI.Droid.Utils;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Activities
{
    public class BaseFragmentActivity<TViewModel> : MvxActivity<TViewModel>
        where TViewModel : class, IMvxViewModel
    {
        private InputMethodManager InputMethodManager => (InputMethodManager) GetSystemService(InputMethodService);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            int? style = ThemeUtils.GetStyleForTheme(AppSettings.SelectedColorTheme, false);
            
            if (style.HasValue)
                SetTheme(style.Value);
        }

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
