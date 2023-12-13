using BMM.Core.Implementations.Security;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.Parameters;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels;

public class WebBrowserViewModel : BaseViewModel<IWebBrowserPrepareParams>, IWebBrowserViewModel
{
    // private IPrepareJsHandlersForWebViewAction _prepareJsHandlersForWebViewAction;

    private readonly IAccessTokenProvider _accessTokenProvider;
    private string _url;
    private string _title;

    public string Script =>
        "(function() {window.xamarin_webview = {\r\n  callHandler(handlerName, ...args) {\r\n    return \"" +
        _accessTokenProvider.AccessToken +
        "\";\r\n  }\r\n}\r\nreturn 1;\r\n  })()";

    public WebBrowserViewModel(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public bool PageLoaded { get; set; }
    public Func<Task<bool>> RequestCloseConfirmation { get; protected set; }
    public Func<Task<bool>> CustomCloseAction { get; protected set; }
    public IBmmInteraction<string> EvaluateJavaScriptInteraction { get; } = new BmmInteraction<string>();
    public IList<string> ScriptsToEvaluateAfterPageLoaded { get; } = new List<string>();
    public IMvxAsyncCommand CloseBrowserCommand { get; protected set; }
    public IDictionary<string, Action<string>> JavaScriptEventHandlers { get; protected set; }

    public override Task Initialize()
    {
        Url = NavigationParameter.Url;
        Title = NavigationParameter.Title;
        JavaScriptEventHandlers = new Dictionary<string, Action<string>>()
        {
            {
                "openQuestionSubmission", async s =>
                {
                    await NavigationService.Navigate<AskQuestionViewModel>();
                }
            }
        };
        
        ScriptsToEvaluateAfterPageLoaded.Add("window.xamarin_webview = {\n    accessToken: 'dupa',\n};");
            
        if (!PageLoaded)
            RaisePropertyChanged(nameof(Url));

        CloseBrowserCommand = new MvxAsyncCommand(
            async () =>
            {
                bool shouldClose = RequestCloseConfirmation == null || await RequestCloseConfirmation();

                if (shouldClose)
                {
                    bool handled = false;

                    if (CustomCloseAction != null)
                        handled = await CustomCloseAction();

                    if (!handled)
                        await CloseCommand.ExecuteAsync();
                }
            });

        return base.Initialize();
    }
}
