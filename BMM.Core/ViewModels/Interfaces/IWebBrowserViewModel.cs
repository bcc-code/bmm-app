using BMM.Core.Interactions.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces;

public interface IWebBrowserViewModel : IMvxViewModel
{
    string Url { get; }
    string Title { get; }
    IDictionary<string, Action<string>> JavaScriptEventHandlers { get; }
    IBmmInteraction<string> EvaluateJavaScriptInteraction { get; }
    IList<string> ScriptsToEvaluateAfterPageLoaded { get; }
}