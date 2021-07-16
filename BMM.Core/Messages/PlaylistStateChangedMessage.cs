using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class PlaylistStateChangedMessage : MvxMessage
    {
        public PlaylistStateChangedMessage(object sender, int id) : base(sender)
        {
            Id = id;
        }

        public int Id { get; }
    }
}