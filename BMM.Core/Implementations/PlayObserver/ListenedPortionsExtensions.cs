using System.Collections.Generic;
using BMM.Core.Implementations.PlayObserver.Model;

namespace BMM.Core.Implementations.PlayObserver
{
    public static class ListenedPortionsExtensions
    {
        public static IList<ListenedPortion> Clone(this IList<ListenedPortion> portions)
        {
            var clone = new List<ListenedPortion>();
            clone.AddRange(portions);
            return clone;
        }
    }
}