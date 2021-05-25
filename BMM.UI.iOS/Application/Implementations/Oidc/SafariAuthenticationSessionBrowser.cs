using System.Threading;
using System.Threading.Tasks;
using Foundation;
using IdentityModel.OidcClient.Browser;
using SafariServices;

namespace BMM.UI.iOS.Implementations
{
    public class SafariAuthenticationSessionBrowser : IBrowser
    {
        private SFAuthenticationSession _authenticationSession;

        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            var wait = new TaskCompletionSource<BrowserResult>();

            _authenticationSession = new SFAuthenticationSession(
                NSUrl.FromString(options.StartUrl),
                NSUrl.FromString(options.EndUrl).Scheme,
                (callbackUrl, error) =>
                {
                    if (error != null)
                    {
                        var errorResult = new BrowserResult
                        {
                            ResultType = BrowserResultType.UserCancel
                        };

                        wait.SetResult(errorResult);
                    }
                    else
                    {
                        var result = new BrowserResult
                        {
                            ResultType = BrowserResultType.Success,
                            Response = callbackUrl.AbsoluteString
                        };

                        wait.SetResult(result);
                    }
                });

            _authenticationSession.Start();
            return wait.Task;
        }
    }
}