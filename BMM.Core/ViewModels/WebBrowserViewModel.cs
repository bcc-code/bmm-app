using BMM.Core.Interactions.Base;
using BMM.Core.Models.Parameters;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class WebBrowserViewModel
        : BaseViewModel<IWebBrowserPrepareParams>,
          IWebBrowserViewModel
    {
        // private IPrepareJsHandlersForWebViewAction _prepareJsHandlersForWebViewAction;
        public static string Script =
            "(function() {window.xamarin_webview = {\r\n  callHandler(handlerName, ...args) {\r\n    return \"eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik4wTTBNemt3TXpFelFUSTJSVEZDTlRjNU16ZzNNRU5FTURBM016TXhOekF5TlVSR05EZENSZyJ9.eyJodHRwczovL2xvZ2luLmJjYy5uby9jbGFpbXMvcGVyc29uSWQiOjQyNTI2LCJodHRwczovL2xvZ2luLmJjYy5uby9jbGFpbXMvcGVyc29uVWlkIjoiMWRiNjRmMzYtYzYzOS00MTVlLTk4OWUtYjFlYzRiM2IzYTQzIiwiaHR0cHM6Ly9tZW1iZXJzLmJjYy5uby9hcHBfbWV0YWRhdGEiOnsiaGFzTWVtYmVyc2hpcCI6dHJ1ZSwicGVyc29uSWQiOjQyNTI2fSwiaXNzIjoiaHR0cHM6Ly9sb2dpbi5iY2Mubm8vIiwic3ViIjoiYXV0aDB8M2U5Nzg3NDEtNmI1Ni00ZDgzLThkODMtZWI4NGEyZDliMjc2IiwiYXVkIjpbImh0dHBzOi8vYm1tLWFwaS5icnVuc3RhZC5vcmciLCJodHRwczovL2JjYy1zc28uZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTY4NzQ0NDM2NSwiZXhwIjoxNjg3NTMwNzY1LCJhenAiOiJhdE5uVzdOMTEzTE9FRm5UQk9McFp2WHVlTGtubTR1RSIsInNjb3BlIjoib3BlbmlkIHByb2ZpbGUgZW1haWwgb2ZmbGluZV9hY2Nlc3MifQ.lmuwlLRJykOLe5XM8poMAu_gDtP8Sa-tcTPjJZ3Himns7Lazxt2tB-kMvxgCzDg0VkTB_zlTygM66szdaVQ2rQQrdLF97QjAw7DUZlIRERD4jwGs3bhgCYMUpHKIVP623xlAiElnofCRZnwa4oYRLPluS0VK5jGYtD8Bd1wEt8YdgsECVCsh18fgAciici3zzfiDCRQ34PT86VJfr5qYXP4EiFljJCOnu1Ub10gPfpbjMh8j0IbFGpxoYODNy7yqDqBLyldMClwtfXNNzuk_3-V0MuVujZM5bEO6BTcrFt_tv85cPY0IBTv7hz4KtavrTFEbBIh2ptfArVEK-_BMUw\";\r\n  }\r\n}\r\nreturn 1;\r\n            })()";
        public string Url { get; set; }
        public bool PageLoaded { get; set; }
        public Func<Task<bool>> RequestCloseConfirmation { get; protected set; }
        public Func<Task<bool>> CustomCloseAction { get; protected set; }
        public IBmmInteraction<string> EvaluateJavaScriptInteraction { get; } = new BmmInteraction<string>();
        public IMvxAsyncCommand CloseBrowserCommand { get; protected set; }
        public IDictionary<string, Action<string>> JavaScriptEventHandlers { get; protected set; }

        public override async Task Initialize()
        {
            Url = NavigationParameter.Url;
            // JavaScriptEventHandlers = await _prepareJsHandlersForWebViewAction.ExecuteGuarded(parameter);

            if (!PageLoaded)
                await RaisePropertyChanged(nameof(Url));

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

            await base.Initialize();
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            // EvaluateJavaScriptInteraction.Raise();
        }
    }
}