using System;
using System.Threading.Tasks;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class BackgroundTaskMessage : MvxMessage
    {
        public BackgroundTaskMessage(object sender, Func<Task> backgroundTask) : base(sender)
        {
            BackgroundTask = backgroundTask;
        }

        public Func<Task> BackgroundTask { get; }
    }
}