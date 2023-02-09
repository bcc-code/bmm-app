using Android.Content;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;

namespace BMM.UI.Droid.Application.Implementations
{
    public class ClipboardService : BaseClipboardService
    {
        public ClipboardService(IToastDisplayer toastDisplayer, IBMMLanguageBinder bmmLanguageBinder) : base(toastDisplayer, bmmLanguageBinder)
        {
        }

        protected override void PlatformCopyToClipboard(string text)
        {
            var clipboard =
                (ClipboardManager) Android.App.Application.Context.GetSystemService(Context.ClipboardService);

            clipboard.PrimaryClip = ClipData.NewPlainText(null, text);
        }
    }
}