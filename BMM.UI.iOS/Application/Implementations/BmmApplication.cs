using Foundation;
using UIKit;
using BMM.UI.iOS.NewMediaPlayer;
using MvvmCross;

namespace BMM.UI.iOS
{
    [Register("BmmApplication")]
    public class BmmApplication : UIApplication
    {
        public override void RemoteControlReceived(UIEvent theEvent)
        {
            Mvx.IoCProvider.Resolve<IMediaRemoteControl>().RemoteControlReceived(theEvent);
        }
    }
}