using BMM.Core.Interactions.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces;

public interface IWebBrowserViewModel : IMvxViewModel
{
    string Url { get; }
    IBmmInteraction<string> EvaluateJavaScriptInteraction { get; } 
}