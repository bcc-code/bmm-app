using System.Threading;
using System.Threading.Tasks;
using AuthenticationServices;
using Foundation;
using IdentityModel.OidcClient.Browser;
using UIKit;

namespace BMM.UI.iOS.Implementations
{
    public class WebAuthenticationSessionBrowser : IBrowser
    {
        private ASWebAuthenticationSession _authenticationSession;

        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            var wait = new TaskCompletionSource<BrowserResult>();

            _authenticationSession = new ASWebAuthenticationSession(
                new NSUrl(options.StartUrl),
                options.EndUrl,
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

            // iOS 13 requires the PresentationContextProvider set
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                _authenticationSession.PresentationContextProvider = new PresentationContextProviderToSharedKeyWindow();

            _authenticationSession.Start();
            return wait.Task;
        }

        private class PresentationContextProviderToSharedKeyWindow : NSObject, IASWebAuthenticationPresentationContextProviding
        {
            public UIWindow GetPresentationAnchor(ASWebAuthenticationSession session)
            {
                return UIApplication.SharedApplication.KeyWindow;
            }
        }
    }
}