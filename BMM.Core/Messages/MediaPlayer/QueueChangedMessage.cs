using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer;

public class QueueChangedMessage : MvxMessage
{
    public QueueChangedMessage(object sender) : base(sender)
    {
    }
}