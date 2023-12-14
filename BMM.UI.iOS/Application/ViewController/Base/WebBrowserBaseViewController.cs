using BMM.Core.Extensions;
using BMM.Core.Interactions.Base;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews;
using BMM.UI.iOS.Delegates;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using Newtonsoft.Json;
using WebKit;

namespace BMM.UI.iOS.ViewController.Base
{
    public abstract class WebBrowserBaseViewController<TViewModel>
        : BaseViewController<TViewModel>, IWKScriptMessageHandler
        where TViewModel : BaseViewModel, IWebBrowserViewModel
    {
        private const int PageRenderingTimeMs = 600;
        private const int WaitingTimeForClose = 100;

        private readonly IDictionary<string, Action<string>> _javaScriptEventHandlers =
            new Dictionary<string, Action<string>>();

        private IBmmInteraction<string> _evaluateJavaScriptInteraction;

        private WKUserContentController _userContentController;
        private UIActivityIndicatorView _activityIndicator;

        protected BmmWebView WebView;
        protected static WkWebViewNavigationDelegate NavigationDelegate;

        private string _url;

        private bool _pageLoaded;
        private bool _showProgressBarBeforeNavigation;
        private UIBarButtonItem _closeButton;
        private IList<string> _scriptsToEvaluateAfterPageLoaded;
        private string _pageTitle;

        protected WebBrowserBaseViewController(string nibName) : base(nibName)
        {
        }

        protected virtual string Script => null;

        protected BmmWebView InitializeWebView()
        {
            _userContentController = new WKUserContentController();
            var webViewConfiguration = new WKWebViewConfiguration();
            webViewConfiguration.UserContentController = _userContentController;

            var webView = WebView ?? new BmmWebView(WebBrowserContainer.Frame, webViewConfiguration);
            webView.BackgroundColor = UIColor.Clear;
            webView.ScrollView.BackgroundColor = UIColor.Black;
            _activityIndicator = new UIActivityIndicatorView
            {
                Color = UIColor.White,
            };

            NavigationDelegate = new WkWebViewNavigationDelegate();
            NavigationDelegate.NavigationFinished += NavigationDelegateOnNavigationFinished;
            webView.NavigationDelegate = NavigationDelegate;
            webView.AllowsBackForwardNavigationGestures = true;

            return webView;
        }

        protected abstract UIView WebBrowserContainer { get; }

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                LoadRequest();
            }
        }

        public IDictionary<string, Action<string>> JavaScriptEventHandlers { get; set; }

        public bool PageLoaded
        {
            get => _pageLoaded;
            set
            {
                _pageLoaded = value;
                OnPropertyChanged();
            }
        }

        protected virtual bool ShouldNavigate => true;
        protected virtual bool ShouldExecuteScript => PageLoaded;

        private void LoadRequest()
        {
            if (string.IsNullOrWhiteSpace(_url) || WebView == null || !ShouldNavigate)
                return;

            try
            {
                var uri = new Uri(_url);
                WebView.LoadRequest(new NSUrlRequest(new NSUrl(uri.AbsoluteUri)));
                _activityIndicator.StartAnimating();
                WebView.Hidden = true;
                WebView.Opaque = false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}{Environment.NewLine}Url: {_url}", e);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AddJavaScriptEventHandlers();
            ConfigureWebView();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AddJavaScriptEventHandlers();
        }

        protected void ConfigureWebView()
        {
            var webBrowserSubviews = WebBrowserContainer.Subviews.ToList();

            if (webBrowserSubviews.Contains(WebView))
                return;

            WebBrowserContainer.BackgroundColor = UIColor.Black;
            WebBrowserContainer.AddSubview(WebView);
            WebBrowserContainer.AddSubview(_activityIndicator);
            ConfigureConstraints(WebView);
        }

        private void AddJavaScriptEventHandlers()
        {
            if (JavaScriptEventHandlers is null)
                return;

            foreach (var singleEvent in JavaScriptEventHandlers)
                AddNewJavaScriptEventToHandle(singleEvent.Key, singleEvent.Value);
        }

        public override void ViewDidLoad()
        {
            WebView = InitializeWebView();
            AttachPageLoadedChangeToNaviDelegate();
            
            base.ViewDidLoad();
            
            var closeButton = new UIBarButtonItem(
                UIImage.FromBundle("IconRemove"),
                UIBarButtonItemStyle.Plain,
                (_, _) => ViewModel?.CloseCommand?.Execute())
            {
                Title = string.Empty,
                TintColor = AppColors.LabelOneColor
            };
            
            NavigationItem.RightBarButtonItem = closeButton;
            
            if (Script != null)
                _userContentController.AddUserScript(new WKUserScript(new NSString(Script),
                    WKUserScriptInjectionTime.AtDocumentStart,
                    false));
            
            Bind();
            LoadRequest();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (!WebBrowserContainer.Subviews.AsEnumerable().Contains(WebView))
                return;

            WebView.RemoveFromSuperview();
            _activityIndicator.RemoveFromSuperview();
        }

        private void RemoveJavaScriptEventHandlers()
        {
            if (JavaScriptEventHandlers is null)
                return;

            foreach (var singleEvent in JavaScriptEventHandlers)
                WebView.RemoveScriptMessageHandler(singleEvent.Key);
        }

        public override void ViewWillDisappear(bool animated)
        {
            RemoveJavaScriptEventHandlers();
            base.ViewWillDisappear(animated);
        }

        private void ConfigureConstraints(UIView view)
        {
            WebBrowserContainer.TranslatesAutoresizingMaskIntoConstraints = false;
            view.TranslatesAutoresizingMaskIntoConstraints = false;
            _activityIndicator.TranslatesAutoresizingMaskIntoConstraints = false;

            WebBrowserContainer.AddConstraint(
                NSLayoutConstraint.Create(
                    view,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.Equal,
                    WebBrowserContainer,
                    NSLayoutAttribute.Height,
                    1,
                    0));
            WebBrowserContainer.AddConstraint(
                NSLayoutConstraint.Create(
                    view,
                    NSLayoutAttribute.Width,
                    NSLayoutRelation.Equal,
                    WebBrowserContainer,
                    NSLayoutAttribute.Width,
                    1,
                    0));
            WebBrowserContainer.AddConstraint(
                NSLayoutConstraint.Create(
                    view,
                    NSLayoutAttribute.CenterX,
                    NSLayoutRelation.Equal,
                    WebBrowserContainer,
                    NSLayoutAttribute.CenterX,
                    1,
                    0));
            WebBrowserContainer.AddConstraint(
                NSLayoutConstraint.Create(
                    view,
                    NSLayoutAttribute.CenterY,
                    NSLayoutRelation.Equal,
                    WebBrowserContainer,
                    NSLayoutAttribute.CenterY,
                    1,
                    0));

            WebBrowserContainer.AddConstraint(
                NSLayoutConstraint.Create(
                    _activityIndicator,
                    NSLayoutAttribute.CenterX,
                    NSLayoutRelation.Equal,
                    WebBrowserContainer,
                    NSLayoutAttribute.CenterX,
                    1,
                    0));
            WebBrowserContainer.AddConstraint(
                NSLayoutConstraint.Create(
                    _activityIndicator,
                    NSLayoutAttribute.CenterY,
                    NSLayoutRelation.Equal,
                    WebBrowserContainer,
                    NSLayoutAttribute.CenterY,
                    1,
                    0));
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<BaseViewController<TViewModel>, TViewModel>();

            set.Bind(this)
                .For(v => v.Url)
                .To(vm => vm.Url);
            
            set.Bind(this)
                .For(p => p.EvaluateJavaScriptInteraction)
                .To(vm => vm.EvaluateJavaScriptInteraction);
            
            set.Bind(this)
                .For(p => p.JavaScriptEventHandlers)
                .To(vm => vm.JavaScriptEventHandlers);
            
            set.Bind(this)
                .For(p => p.ScriptsToEvaluateAfterPageLoaded)
                .To(vm => vm.ScriptsToEvaluateAfterPageLoaded);
            
            set.Bind(this)
                .For(v => v.PageTitle)
                .To(vm => vm.Title);
            
            set.Apply();
        }

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                Title = _pageTitle;
            }
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

        public bool ShowProgressBarBeforeNavigation
        {
            get => _showProgressBarBeforeNavigation;
            set
            {
                _showProgressBarBeforeNavigation = value;

                if (!value)
                    return;

                WebView.Hidden = true;
                _activityIndicator?.StartAnimating();
            }
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            DetachPageLoadedChangeToNaviDelegate();
        }

        protected void AttachPageLoadedChangeToNaviDelegate()
        {
            NavigationDelegate.NavigationFinished += OnNavigationDelegateOnNavigationFinished;
            NavigationDelegate.NavigationStarted += NavigationDelegateOnNavigationStarted;
        }

        private void NavigationDelegateOnNavigationStarted(object sender, bool e)
        {
            _activityIndicator.StartAnimating();
            WebView.UserInteractionEnabled = false;
        }

        protected void DetachPageLoadedChangeToNaviDelegate()
        {
            NavigationDelegate.NavigationFinished -= OnNavigationDelegateOnNavigationFinished;
            NavigationDelegate.NavigationStarted -= NavigationDelegateOnNavigationStarted;
        }

        private void OnNavigationDelegateOnNavigationFinished(object sender, bool b)
        {
            WebView.IsInitialPageLoaded = true;
            WebView.ShouldExecuteScript = true;
            PageLoaded = true;
            WebView.RunEvaluation().FireAndForget();
        }

        protected virtual async void NavigationDelegateOnNavigationFinished(object sender, bool e)
        {
            await Task.Delay(WaitingTimeForClose);
            _activityIndicator.StopAnimating();
            WebView.UserInteractionEnabled = true;
            WebView.Hidden = false;

            await Task.Delay(PageRenderingTimeMs);
            WebView.Opaque = true;
        }

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

        private void AddNewJavaScriptEventToHandle(string eventName, Action<string> handler)
        {
            _javaScriptEventHandlers.Remove(eventName);
            _javaScriptEventHandlers.Add(eventName, handler);
            WebView.SafeAddScriptMessageHandler(this, eventName);
        }

        private void EvaluateJavaScriptInteractionOnRequested(object sender, MvxValueEventArgs<string> e)
            => WebView.EnqueueScriptToExecute(e.Value);

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            string formattedMessage = CreateMessageFromBody(message.Body);

            if (_javaScriptEventHandlers.TryGetValue(message.Name, out var eventHandler))
                eventHandler.Invoke(formattedMessage);
        }

        private static string CreateMessageFromBody(NSObject message)
        {
            if (!(message is NSDictionary payloadDictionary))
                return message?.ToString();

            var dict = payloadDictionary
                .ToDictionary(
                    item => item.Key.ToString(),
                    item => item.Value.ToString());

            return JsonConvert.SerializeObject(dict);
        }
    }
}