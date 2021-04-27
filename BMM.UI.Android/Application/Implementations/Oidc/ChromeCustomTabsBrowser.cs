using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Browser.CustomTabs;
using AndroidX.Core.Content;
using BMM.Core.Implementations.Security.Oidc;
using IdentityModel.OidcClient.Browser;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.Oidc
{
    public class ChromeCustomTabsBrowser : IBrowser
    {
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            var context = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            var manager = new CustomTabsActivityManager(context);
            var task = new TaskCompletionSource<BrowserResult>();

            var builder = new CustomTabsIntent.Builder(manager.Session)
                .SetToolbarColor(ContextCompat.GetColor(context, Resource.Color.bcc_login))
                .SetShowTitle(true)
                .EnableUrlBarHiding();

            var customTabsIntent = builder.Build();

            // ensures the intent is not kept in the history stack, which makes
            // sure navigating away from it will close it
            customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);

            void Callback(string response)
            {
                OidcCallbackMediator.Instance.CallbackMessageReceived -= Callback;

                var cancelled = response == "UserCancel";
                task.SetResult(new BrowserResult
                {
                    ResultType = cancelled ? BrowserResultType.UserCancel : BrowserResultType.Success,
                    Response = response
                });
            }

            OidcCallbackMediator.Instance.CallbackMessageReceived += Callback;

            customTabsIntent.LaunchUrl(context, Android.Net.Uri.Parse(options.StartUrl));

            return task.Task;
        }
    }
}