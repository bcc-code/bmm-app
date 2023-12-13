using Acr.UserDialogs;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Webkit;
using BMM.Core.Extensions;
using BMM.Core.Interactions.Base;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.UI.Droid.Application.CustomViews;
using Java.Interop;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.Fragments.Base
{
    public abstract class WebBrowserBaseFragment<TViewModel>
        : BaseDialogFragment<TViewModel>,
          IValueCallback
        where TViewModel : BaseViewModel, IWebBrowserViewModel
    {
        private const int PossiblePageRenderingTimeMs = 600;
        private const int WaitingTimeForClosingPreviousPage = 100;
        protected static ProgressBar Progress;

        private IBmmInteraction<string> _evaluateJavaScriptInteraction;
        private IDictionary<string, Action<string>> _javaScriptEventHandlers;
        private bool _pageLoaded;

        private string _url;

        protected BmmWebView WebView;
        private string _pageTitle;
        private IList<string> _scriptsToEvaluateAfterPageLoaded;

        protected FrameLayout WebViewContainer => FragmentView.FindViewById<FrameLayout>(WebViewContainerId);

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                if (!string.IsNullOrWhiteSpace(_url))
                {
                    if (WebView == null || !ShouldNavigate)
                        return;

                    WebView.LoadUrl(_url);
                    WebView.Visibility = ViewStates.Invisible;

                    if (Progress != null)
                        Progress.Visibility = ViewStates.Visible;
                }
            }
        }

        protected virtual bool ShouldExecuteScript => PageLoaded;
        protected virtual bool ShouldNavigate => true;

        public bool PageLoaded
        {
            get => _pageLoaded;
            set
            {
                _pageLoaded = value;
                OnPropertyChanged();
            }
        }

        protected abstract int WebViewContainerId { get; }
        protected abstract bool ShouldWebViewInterceptTouches { get; }

        public IBmmInteraction<string> EvaluateJavaScriptInteraction
        {
            get => _evaluateJavaScriptInteraction;
            set
            {
                if (_evaluateJavaScriptInteraction != null)
                    _evaluateJavaScriptInteraction.Requested -= EvaluateJavaScriptInteractionOnRequested;

                _evaluateJavaScriptInteraction = value;
                _evaluateJavaScriptInteraction.Requested += EvaluateJavaScriptInteractionOnRequested;
            }
        }

        private void EvaluateJavaScriptInteractionOnRequested(object sender, MvxValueEventArgs<string> e)
            => WebView.EnqueueScriptToExecute(e.Value);

        void IValueCallback.OnReceiveValue(Object value)
        {
            //implementation to evaluating JS on WebView
        }

        public virtual void EnsureWebViewIsSet() => WebView = InitializeWebView();

        protected void InjectWebView(BmmWebView webView)
        {
            WebView = webView;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            EnsureWebViewIsSet();

            ClearParent();

            Progress = new ProgressBar(Context);

            if (WebView.IsInitialPageLoaded)
            {
                WebView.Visibility = ViewStates.Visible;
                Progress.Visibility = ViewStates.Invisible;
            }

            WebViewContainer?.AddView(WebView);
            WebViewContainer?.AddView(
                Progress,
                new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent,
                    GravityFlags.Center));

            var set = this.CreateBindingSet<WebBrowserBaseFragment<TViewModel>, TViewModel>();
            Bind(set);
            set.Apply();

            return view;
        }

        protected virtual void Bind(MvxFluentBindingDescriptionSet<WebBrowserBaseFragment<TViewModel>, TViewModel> set)
        {
            set.Bind(this)
                .For(v => v.Url)
                .To(vm => vm.Url);
            
            set.Bind(this)
                .For(v => v.PageTitle)
                .To(vm => vm.Title);

            set.Bind(this)
                .For(p => p.EvaluateJavaScriptInteraction)
                .To(vm => vm.EvaluateJavaScriptInteraction);
            
            set.Bind(this)
                .For(p => p.ScriptsToEvaluateAfterPageLoaded)
                .To(vm => vm.ScriptsToEvaluateAfterPageLoaded);
        }

        public IList<string> ScriptsToEvaluateAfterPageLoaded
        {
            get => _scriptsToEvaluateAfterPageLoaded;
            set
            {
                _scriptsToEvaluateAfterPageLoaded = value;
                
                foreach (string script in _scriptsToEvaluateAfterPageLoaded)
                    WebView.EnqueueScriptToExecute(script);
            }
        }
        
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                Toolbar.Title = _pageTitle;
            }
        }

        protected BmmWebView InitializeWebView(BmmWebView webView = null)
        {
            if (webView == null)
                webView = new BmmWebView(Context);

            if (!webView.IsClientSet)
                webView.SetWebViewClient(new EventNotifyingWebViewClient());

            if (webView.WebViewClient is EventNotifyingWebViewClient eventNotifyingWebViewClient)
            {
                eventNotifyingWebViewClient.NavigationStarted += ClientOnNavigationStarted;
                eventNotifyingWebViewClient.NavigationFinished += ClientOnNavigationFinished;
            }

            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.SetBackgroundColor(Color.Black);
            webView.ShouldInterceptTouch = ShouldWebViewInterceptTouches;
            webView.AddJavascriptInterface(new MyJSInterface(Context), "android");

            return webView;
        }

        private void ClientOnNavigationStarted(object sender, EventArgs e)
        {
            WebView.Visibility = ViewStates.Invisible;
            Progress.Visibility = ViewStates.Visible;
        }

        private void HideProgressAndShowWebview()
        {
            WebView.Visibility = ViewStates.Visible;
            Progress.Visibility = ViewStates.Gone;
        }

        private async void ClientOnNavigationFinished(object sender, bool pageLoadedSuccessfully)
        {
            if (!pageLoadedSuccessfully)
                return;

            await Task.Delay(WaitingTimeForClosingPreviousPage);
            WebView.Visibility = ViewStates.Visible;

            PageLoaded = pageLoadedSuccessfully;
            WebView.IsInitialPageLoaded = true;
            WebView.ShouldExecuteScript = true;
            WebView
                .RunEvaluation()
                .FireAndForget();

            await Task.Delay(PossiblePageRenderingTimeMs);
            if (Progress != null)
                Progress.Visibility = ViewStates.Gone;

            WebView.SetBackgroundColor(Color.White);
        }

        public override void OnDetach()
        {
            base.OnDetach();
            Clear();
        }

        private void Clear()
        {
            if (WebView?.Parent != WebViewContainer || WebViewContainer == null)
                return;

            WebViewContainer.RemoveView(WebView);
            WebViewContainer.RemoveView(Progress);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            if (WebView is null)
                return;

            if (!(WebView.WebViewClient is EventNotifyingWebViewClient eventNotifyingWebViewClient))
                return;

            eventNotifyingWebViewClient.NavigationStarted -= ClientOnNavigationStarted;
            eventNotifyingWebViewClient.NavigationFinished -= ClientOnNavigationFinished;
        }

        private void ClearParent()
        {
            if (!(WebView?.Parent is ViewGroup viewGroup))
                return;

            viewGroup.RemoveView(WebView);
            viewGroup.RemoveView(Progress);
        }
    }
    
    public class MyJSInterface : Java.Lang.Object
    {
        Context mContext;

        public MyJSInterface(Context context)
        {
            mContext = context;
        }

        [JavascriptInterface]
        [Export("openQuestionSubmission")]
        public void OpenQuestionSubmission()
        {
            Mvx.IoCProvider.Resolve<IUserDialogs>().Toast("asdad");
        }
    }
}