using UIKit;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public interface IMediaRemoteControl
    {
        void RemoteControlReceived(UIEvent uiEvent);
    }
}