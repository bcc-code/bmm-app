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
                {
                    return i;
                }
            }

            return NumericConstants.Undefined;;
        }
    }
}
