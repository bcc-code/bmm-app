using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

namespace BMM.UI.Droid.Application.Helpers
{
    // https://stackoverflow.com/questions/1109022/how-do-you-close-hide-the-android-soft-keyboard-using-java
    public static class HideKeyboardExtensions
    {
        public static void HideKeyboard(this Activity activity)
        {
            var inputManager = activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
            var view = activity.CurrentFocus ?? new View(activity);

            inputManager.HideSoftInputFromWindow(view.WindowToken, 0);
        }

        public static void HideKeyboardForView(this AndroidX.Fragment.App.Fragment fragment, View view)
        {
            var inputManager = fragment.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputManager.HideSoftInputFromWindow(view.WindowToken, 0);
        }
    }
}