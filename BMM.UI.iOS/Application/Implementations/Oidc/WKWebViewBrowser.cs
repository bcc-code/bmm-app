using System;
using System.Threading;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Security.Oidc;
using Foundation;
using IdentityModel.OidcClient.Browser;
using UIKit;
using WebKit;

namespace BMM.UI.iOS.Implementations
{
    public class WKWebViewBrowser: IBrowser
    {
        private WKWebView _webView;

        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            var expectedResult = new TaskCompletionSource<BrowserResult>();
            var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            var configuration = new WKWebViewConfiguration();
            _webView = new WKWebView(rootViewController.View.Bounds, configuration) {NavigationDelegate = new OidcWkNavigationDelegate()};
            _webView.TranslatesAutoresizingMaskIntoConstraints = false;

            rootViewController.View.AddSubview(_webView);
            _webView.TopAnchor.ConstraintEqualTo(rootViewController.View.TopAnchor).Active = true;
            _webView.LeadingAnchor.ConstraintEqualTo(rootViewController.View.LeadingAnchor).Active = true;
            _webView.TrailingAnchor.ConstraintEqualTo(rootViewController.View.TrailingAnchor).Active = true;
            _webView.BottomAnchor.ConstraintEqualTo(rootViewController.View.BottomAnchor).Active = true;
            _webView.LoadRequest(new NSUrlRequest(new NSUrl(options.StartUrl)));

            void Callback(string response)
            {
                OidcCallbackMediator.Instance.CallbackMessageReceived -= Callback;

                if (response == "UserCancel")
                {
                    expectedResult.SetResult(new BrowserResult
                    {
                        ResultType = BrowserResultType.UserCancel
                    });
                }
                else
                {
                    _webView.RemoveFromSuperview();
                    _webView.Dispose();
                    expectedResult.SetResult(new BrowserResult
                    {
                        ResultType = BrowserResultType.Success,
                        Response = response
                    });
                }
            }

            OidcCallbackMediator.Instance.CallbackMessageReceived += Callback;

            return expectedResult.Task;
        }
    }

    public class OidcWkNavigationDelegate : WKNavigationDelegate
    {
        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            var request = navigationAction.Request;
            var incomingUrl = $"{request.Url.Scheme}://{request.Url.Host}";
            if (incomingUrl == OidcConstants.LoginRedirectUrl)
                OidcCallbackMediator.Instance.Send(request.Url.AbsoluteString);

            decisionHandler(WKNavigationActionPolicy.Allow);
        }
    }
}