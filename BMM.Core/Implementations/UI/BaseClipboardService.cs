using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;

namespace BMM.Core.Implementations.UI
{
    public abstract class BaseClipboardService : IClipboardService
    {
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public BaseClipboardService(
            IToastDisplayer toastDisplayer,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _toastDisplayer = toastDisplayer;
            _bmmLanguageBinder = bmmLanguageBinder;
        }
        
        public void CopyToClipboard(string textToCopy, string valueName = "")
        {
            if (!string.IsNullOrEmpty(valueName))
                _toastDisplayer.Success(_bmmLanguageBinder.GetText(Translations.Global_ValueCopiedToClipboard, valueName));
           
            PlatformCopyToClipboard(textToCopy);
        }

        protected abstract void PlatformCopyToClipboard(string text);
    }
}