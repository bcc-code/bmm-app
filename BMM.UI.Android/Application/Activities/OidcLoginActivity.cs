using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.ViewModels;
using IdentityModel.OidcClient.Browser;

namespace BMM.UI.Droid.Application.Activities
{
    [Activity(
        Label = "Login",
        Theme = "@style/AppTheme.Login",
        Name = "bmm.ui.droid.application.activities.OidcLoginActivity",
        ScreenOrientation = ScreenOrientation.Portrait,
        LaunchMode = LaunchMode.SingleTask
    )]
    [IntentFilter(
        new[] {Intent.ActionView},
        AutoVerify = true,
        Categories = new[] {Intent.CategoryDefault, Intent.CategoryBrowsable},
        DataScheme = "org.brunstad.bmm",
        DataHosts = new[] {"login-callback"}
    )]
    public class OidcLoginActivity : BaseFragmentActivity<OidcLoginViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_oidc_login);

            if (ViewModel.IsInitialLogin)
                ViewModel.LoginCommand.Execute();
        }

        /// <summary>
        /// Method executed when the Activity resumes that will cancel any pending
        /// <see cref="IBrowser"/> implementation by way of the <see cref="OidcCallbackMediator"/>.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            OidcCallbackMediator.Instance.Cancel();
        }

        /// <summary>
        /// Method executed when the Activity receives a new intent that may continue a pending
        /// <see cref="IBrowser"/> implementation by way of the <see cref="OidcCallbackMediator"/>.
        /// </summary>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            OidcCallbackMediator.Instance.Send(intent.DataString);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

}