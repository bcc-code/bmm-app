using BMM.Core.Implementations.Localization.Interfaces;

namespace BMM.Core.Implementations.Localization
{
    public static class BMMLanguageBinderLocator
    {
        public static IBMMLanguageBinder TextSource { get; private set; }

        public static void SetImplementation(IBMMLanguageBinder languageBinder)
        {
            TextSource = languageBinder;
        }

        public static void ClearImplementation()
        {
            TextSource = null;
        }
    }
}