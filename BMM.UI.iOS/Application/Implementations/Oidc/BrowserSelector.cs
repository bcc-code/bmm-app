using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Browser;
using UIKit;

namespace BMM.UI.iOS.Implementations
{
    public class BrowserSelector : IBrowser
    {
        private readonly IBrowser _browser;

        public BrowserSelector()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                _browser = new WebAuthenticationSessionBrowser();

            else if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                _browser = new SafariAuthenticationSessionBrowser();

            else
                _browser = new SafariViewControllerBrowser();

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("USE_UI_TEST_OIDC_BROWSER")))
                _browser = new WKWebViewBrowser();
        }

        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _browser.InvokeAsync(options, cancellationToken);
        }
    }
}