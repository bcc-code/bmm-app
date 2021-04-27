using BMM.Api.Implementation.Models;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class ListeningStreakChangedMessage : MvxMessage
    {
        public ListeningStreak ListeningStreak { get; set; }

        public ListeningStreakChangedMessage(object sender) : base(sender)
        { }
    }
}
