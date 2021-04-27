using System.Threading;
using System.Threading.Tasks;
using BMM.Core.Implementations.Security.Oidc;
using Foundation;
using IdentityModel.OidcClient.Browser;
using SafariServices;
using UIKit;

namespace BMM.UI.iOS.Implementations
{
    public class SafariViewControllerBrowser : IBrowser
    {
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            // Create Safari controller
            var safari = new SFSafariViewController(new NSUrl(options.StartUrl))
            {
                Delegate = new SafariViewControllerDelegate()
            };

            async void Callback(string response)
            {
                OidcCallbackMediator.Instance.CallbackMessageReceived -= Callback;

                if (response == "UserCancel")
                {
                    tcs.SetResult(new BrowserResult
                    {
                        ResultType = BrowserResultType.UserCancel
                    });
                }
                else
                {
                    await safari.DismissViewControllerAsync(true); // Close Safari
                    safari.Dispose();
                    tcs.SetResult(new BrowserResult
                    {
                        ResultType = BrowserResultType.Success,
                        Response = response
                    });
                }
            }

            OidcCallbackMediator.Instance.CallbackMessageReceived += Callback;

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(safari, true, null);

            return tcs.Task;
        }

        private class SafariViewControllerDelegate : SFSafariViewControllerDelegate
        {
            public override void DidFinish(SFSafariViewController controller)
            {
                OidcCallbackMediator.Instance.Send("UserCancel");
            }
        }
    }
}