using BMM.Core.Interactions.Base;
using BMM.Core.Models.Parameters;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels;

public class WebBrowserViewModel : BaseViewModel<IWebBrowserPrepareParams>, IWebBrowserViewModel
{
    // private IPrepareJsHandlersForWebViewAction _prepareJsHandlersForWebViewAction;

    public string Url { get; set; }
    public bool PageLoaded { get; set; }
    public Func<Task<bool>> RequestCloseConfirmation { get; protected set; }
    public Func<Task<bool>> CustomCloseAction { get; protected set; }
    public IBmmInteraction<string> EvaluateJavaScriptInteraction { get; } = new BmmInteraction<string>();
    public IMvxAsyncCommand CloseBrowserCommand { get; protected set; }
    public IDictionary<string, Action<string>> JavaScriptEventHandlers { get; protected set; }

    public override Task Initialize()
    {
        Url = NavigationParameter.Url;
        // JavaScriptEventHandlers = await _prepareJsHandlersForWebViewAction.ExecuteGuarded(parameter);
            
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
