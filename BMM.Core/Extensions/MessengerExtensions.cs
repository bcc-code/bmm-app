using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Extensions
{
    public static class MessengerExtensions
    {
        public static void UnsubscribeSafe<TMessage>(this IMvxMessenger mvxMessenger, MvxSubscriptionToken token)
            where TMessage : MvxMessage
        {
            if (mvxMessenger == null || token == null)
                return;
            
            mvxMessenger.Unsubscribe<TMessage>(token);
        }
    }
}