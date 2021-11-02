using System.Collections.Generic;
using BMM.Api.Abstraction;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    public class CurrentQueueChangedMessage : MvxMessage
    {
        public CurrentQueueChangedMessage(IList<IMediaTrack> queue, object sender) : base(sender)
        {
            Queue = queue;
        }

        public IList<IMediaTrack> Queue { get; }
    }
}