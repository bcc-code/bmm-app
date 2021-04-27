using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Browser;

namespace BMM.UI.Droid.Application.Implementations.Oidc
{
    public class BrowserSelector : IBrowser
    {
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            IBrowser browser;
            if (BmmApplication.RunsUiTest)
                browser = new WebkitWebViewBrowser();
            else
                browser = new ChromeCustomTabsBrowser();

            return browser.InvokeAsync(options, cancellationToken);
        }
    }
}