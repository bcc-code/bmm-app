using BMM.Core.Implementations.UI;
using UIKit;

namespace BMM.UI.iOS.Implementations
{
    public class ClipboardService : IClipboardService
    {
        public void CopyToClipboard(string text)
        {
            UIPasteboard.General.String = text;
        }
    }
}
