using Android.App;
using Android.Content.PM;
using Android.OS;
using BMM.Core.ViewModels;

namespace BMM.UI.Droid.Application.Activities
{
    [Activity(
        Label = "Loading",
        Theme = "@style/AppTheme.Login",
        Name = "bmm.ui.droid.application.activities.UserSetupActivity",
        ScreenOrientation = ScreenOrientation.Portrait,
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTask
    )]
    public class UserSetupActivity : BaseFragmentActivity<UserSetupViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_user_setup);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}