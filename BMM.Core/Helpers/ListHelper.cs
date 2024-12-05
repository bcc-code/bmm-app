using System;
using System.Collections.Generic;
using BMM.Core.Constants;

namespace BMM.Core.Helpers
{
    public static class ListHelper
    {
        public static int FindIndex<T>(this IList<T> source, Predicate<T> match, int startIndex = 0)
        {
            for (int i = startIndex; i < source.Count; i++)
            {
                if (match(source[i]))
                    return i;
            }

            return NumericConstants.Undefined;
        }
        
        public static T FindNextAfter<T>(this IList<T> source, T item)
        {
            int nextIndex = source
                .IndexOf(item) + 1;

            if (nextIndex >= source.Count)
                return default;

            return source[nextIndex];
        }
        
        public static T FindPreviousBefore<T>(this IList<T> source, T item)
        {
            int previousIndex = source
                .IndexOf(item) - 1;

            if (previousIndex < NumericConstants.Zero)
                return default;

            return source[previousIndex];
        }
    }
}
