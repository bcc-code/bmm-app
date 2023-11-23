using WebKit;

namespace BMM.UI.iOS.Delegates
{
    public class WkWebViewNavigationDelegate : WKNavigationDelegate
    {
        public event EventHandler<bool> NavigationFinished;
        public event EventHandler<bool> NavigationStarted;

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            NavigationFinished?.Invoke(webView, true);
        }

        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            NavigationStarted?.Invoke(webView, true);
        }

        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            NavigationFinished?.Invoke(webView, false);
        }

        public override void DecidePolicy(
            WKWebView webView,
            WKNavigationAction navigationAction,
            Action<WKNavigationActionPolicy> decisionHandler)
        {
            bool handled = HandleOpenPdfWithoutSslIfNeeded(webView, navigationAction);
            decisionHandler(
                handled
                    ? WKNavigationActionPolicy.Cancel
                    : WKNavigationActionPolicy.Allow);
        }

        private bool HandleOpenPdfWithoutSslIfNeeded(WKWebView webView, WKNavigationAction navigationAction)
        {
            string absoluteString = navigationAction.Request.Url.AbsoluteString;

            if (absoluteString == webView.Url?.AbsoluteString
                || navigationAction.NavigationType != WKNavigationType.LinkActivated
                || !(navigationAction.TargetFrame?.MainFrame ?? true))
                return false;

            var request = new NSUrlRequest(new NSUrl(navigationAction.Request.Url.AbsoluteString));
            webView.LoadRequest(request);

            return true;
        }
    }
}