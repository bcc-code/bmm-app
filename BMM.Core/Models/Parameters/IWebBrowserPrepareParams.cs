namespace BMM.Core.Models.Parameters;

public interface IWebBrowserPrepareParams
{
    string Title { get; }
    string Url { get; }
    IDictionary<string, Action<string>> JavaScriptEventHandlers { get; }
}