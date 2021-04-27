using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Helpers
{
    public class TrackEqualityComparer : IEqualityComparer<Track>
    {
        public bool Equals(Track x, Track y)
        {
            return x.GetUniqueKey.Equals(y.GetUniqueKey);
        }

        public int GetHashCode(Track obj)
        {
            return obj.GetHashCode();
        }
    }
}
