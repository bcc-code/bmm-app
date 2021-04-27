using BMM.Core.Implementations.Caching;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class CacheUpdatedMessage : MvxMessage
    {
        public CacheUpdatedMessage(object sender, CacheKeys type) : base(sender)
        {
            Type = type;
        }

        public readonly CacheKeys Type;
    }
}
