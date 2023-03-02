using Android.App;
using Android.Content.PM;
using Android.OS;
using BMM.Core.ViewModels;

namespace BMM.UI.Droid.Application.Activities
{
    [Activity(
        Label = "SupportEndedActivity",
        Theme = "@style/AppTheme.Login",
        Name = "bmm.ui.droid.application.activities.SupportEndedActivity",
        ScreenOrientation = ScreenOrientation.Portrait,
        NoHistory = true
    )]
    public class SupportEndedActivity : BaseFragmentActivity<SupportEndedViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_support_ended);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}