using System.Collections.Generic;

namespace BMM.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> EncloseInIEnumerable<T>(this T source)
        {
            return new[] { source };
        }

        public static T[] EncloseInArray<T>(this T source)
        {
            return new[] { source };
        }
    }
}