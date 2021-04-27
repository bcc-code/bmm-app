using System;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer
{
    [Obsolete]
    public class ShowPlayerViewmodel : MvxMessage
    {
        public ShowPlayerViewmodel(object sender) : base(sender)
        { }
    }
}