using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class TogglePlayerMessage : MvxMessage
    {
        public bool Open { get; }

        public TogglePlayerMessage(object sender, bool open) : base(sender)
        {
            Open = open;
        }
    }
}
