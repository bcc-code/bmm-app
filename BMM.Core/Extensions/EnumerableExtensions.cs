using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public static IList<T> EncloseInList<T>(this T source)
        {
            return new List<T> { source };
        }
        
        public static int LastIndexOfElementType<T>(this IList<T> source, Type elementType, int increaseByIfFound = 0)
        {
            var lastIndex = source
                .LastOrDefault(x => x.GetType() == elementType);
            
            if (lastIndex == null)
                return 0;

            return source.IndexOf(lastIndex) + increaseByIfFound;
        }
        
        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            return list
                .OrderBy(x => Guid.NewGuid())
                .First();
        }
        
        public static T SafeGetAt<T>(this IEnumerable<T> enumerable, int index, T defaultValue)
        {
            if (enumerable.Count() <= index)
                return defaultValue;
            
            return enumerable.ElementAt(index);
        }
    }
}