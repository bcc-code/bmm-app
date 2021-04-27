using Android.Content;
using BMM.Core.Implementations.UI;

namespace BMM.UI.Droid.Application.Implementations
{
    public class ClipboardService : IClipboardService
    {
        public void CopyToClipboard(string text)
        {
            var clipboard =
                (ClipboardManager) Android.App.Application.Context.GetSystemService(Context.ClipboardService);

            clipboard.PrimaryClip = ClipData.NewPlainText(null, text);
        }
    }
}