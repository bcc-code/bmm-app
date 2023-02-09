using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using UIKit;

namespace BMM.UI.iOS.Implementations
{
    public class ClipboardService : BaseClipboardService
    {
        public ClipboardService(IToastDisplayer toastDisplayer, IBMMLanguageBinder bmmLanguageBinder) : base(toastDisplayer, bmmLanguageBinder)
        {
        }

        protected override void PlatformCopyToClipboard(string text)
        {
            UIPasteboard.General.String = text;
        }
    }
}
