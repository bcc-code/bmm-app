using Foundation;
using Intents;
using IntentsUI;

namespace BMM.UI.iOS
{
    public class AddVoiceShortcutViewControllerDelegate : 
        NSObject,
        IINUIAddVoiceShortcutViewControllerDelegate
    {
        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut? voiceShortcut, NSError? error)
        {
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }
    }
}