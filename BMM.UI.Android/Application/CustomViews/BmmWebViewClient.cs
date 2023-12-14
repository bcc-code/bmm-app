using Android.Webkit;

namespace BMM.UI.Droid.Application.CustomViews
{
    public class EventNotifyingWebViewClient : WebViewClient
    {
        private bool _failed;
        public event EventHandler<bool> NavigationFinished;
        public event EventHandler NavigationStarted;

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            NavigationFinished?.Invoke(view, !_failed);
            _failed = false;
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);
            NavigationFinished?.Invoke(view, _failed);
            _failed = true;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            string url = request?.Url?.ToString();

            if (url is null)
                return base.ShouldOverrideUrlLoading(view, request);

            NavigationStarted?.Invoke(view, EventArgs.Empty);
            view.LoadUrl(url);

            return true;
        }
    }
}