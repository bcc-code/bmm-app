using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces;

public interface IWebBrowserViewModel : IMvxViewModel
{
    string Url { get; }
}