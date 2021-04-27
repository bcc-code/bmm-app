using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class MobileNetworkDownloadAllowedChangeMessage: MvxMessage
    {
        public MobileNetworkDownloadAllowedChangeMessage (object sender, bool mobileNetworkDownloadAllowed)
            : base (sender)
        {
            MobileNetworkDownloadAllowed = mobileNetworkDownloadAllowed;
        }

        public readonly bool MobileNetworkDownloadAllowed;
    }
}
