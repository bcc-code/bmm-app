using System.Collections.Generic;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class FollowedPodcastsChangedMessage : MvxMessage
    {
        public FollowedPodcastsChangedMessage(object sender, IEnumerable<int> followedPodcasts)
            : base(sender)
        {
            FollowedPodcasts = followedPodcasts;
        }

        public readonly IEnumerable<int> FollowedPodcasts;
    }
}

