using BMM.Core.Implementations.Localization.Interfaces;
using MvvmCross.Localization;

namespace BMM.Core.Implementations.Localization
{
    public class BMMLanguageBinder : MvxLanguageBinder, IBMMLanguageBinder
    {
        public string this[string key] => GetText(key);
    }
}