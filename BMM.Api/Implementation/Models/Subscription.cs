using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class Subscription
    {
        public string DeviceId { get; set; }

        public IList<PodcastReference> PodcastReferences { get; set; }

        public string Token { get; set; }
        
        public bool ShowNotificationBadge { get; set; }
        public string OS { get; set; }

        public virtual bool ShouldSerializeDeviceId()
        {
            return false;
        }
    }
}