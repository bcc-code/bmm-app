using System.Threading;
using System.Threading.Tasks;
using Android.Views;
using Android.Webkit;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Security.Oidc;
using IdentityModel.OidcClient.Browser;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.Oidc
{
    public class WebkitWebViewBrowser : IBrowser
    {
        private WebView _webView;

        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            var context = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            _webView = new WebView(context);
            _webView.Settings.JavaScriptEnabled = true;
            _webView.LoadUrl(options.StartUrl);
            _webView.SetWebViewClient(new CustomWebViewClient());
            context.SetContentView(_webView);

            var task = new TaskCompletionSource<BrowserResult>();

            void Callback(string response)
            {
                OidcCallbackMediator.Instance.CallbackMessageReceived -= Callback;

                _webView.ClearCache(true);
                var parent = (ViewGroup)_webView.Parent;
                parent.RemoveView(_webView);
                _webView.Dispose();

                var cancelled = response == "UserCancel";
                task.SetResult(new BrowserResult
                {
                    ResultType = cancelled ? BrowserResultType.UserCancel : BrowserResultType.Success,
                    Response = response
                });
            }

            OidcCallbackMediator.Instance.CallbackMessageReceived += Callback;

            return task.Task;
        }

        private class CustomWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().Contains(OidcConstants.LoginRedirectUrl))
                    return true;

                view.LoadUrl(request.Url.ToString());
                return true;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (url.Contains(OidcConstants.LoginRedirectUrl))
                    OidcCallbackMediator.Instance.Send(url);
            }
        }
    }
}