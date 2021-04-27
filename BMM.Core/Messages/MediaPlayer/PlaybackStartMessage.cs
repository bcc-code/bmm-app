using System;
using BMM.Api.Abstraction;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    [Obsolete]
    public class PlaybackStartMessage : MvxMessage
    {
        public PlaybackStartMessage(object sender, IMediaTrack mediaFile) : base(sender)
        {
            CurrentTrack = mediaFile;
        }

        public readonly IMediaTrack CurrentTrack;
    }
}
