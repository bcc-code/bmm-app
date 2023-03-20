using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace BMM.UI.Droid.Application.Activities
{
    [Activity(
        Label = "Login",
        Theme = "@style/AppTheme.Login",
        Name = "bmm.ui.droid.application.activities.OidcLoginActivity",
        ScreenOrientation = ScreenOrientation.Portrait,
        LaunchMode = LaunchMode.SingleTask,
        Exported = true
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
            Platform.Init(this, savedInstanceState);
            
            SetContentView(Resource.Layout.activity_oidc_login);

            if (ViewModel.IsInitialLogin)
                ViewModel.LoginCommand.Execute();
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Method executed when the Activity resumes that will cancel any pending
        /// <see cref="IdentityModel.OidcClient.Browser.IBrowser"/> implementation by way of the <see cref="OidcCallbackMediator"/>.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            OidcCallbackMediator.Instance.Cancel();
        }

        /// <summary>
        /// Method executed when the Activity receives a new intent that may continue a pending
        /// <see cref="IdentityModel.OidcClient.Browser.IBrowser"/> implementation by way of the <see cref="OidcCallbackMediator"/>.
        /// </summary>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            OidcCallbackMediator.Instance.Send(intent.DataString);
        }
    }
}