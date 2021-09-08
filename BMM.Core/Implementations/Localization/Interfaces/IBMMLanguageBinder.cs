using MvvmCross.Localization;

namespace BMM.Core.Implementations.Localization.Interfaces
{
    public interface IBMMLanguageBinder : IMvxLanguageBinder
    {
        string this[string key] { get; }
    }
}