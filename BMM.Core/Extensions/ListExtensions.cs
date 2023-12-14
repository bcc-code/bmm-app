using System;
using System.Collections.Generic;
using System.Linq;

namespace BMM.Core.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfNotNullOrEmpty(this IList<string> list, string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return;
            
            list.Add(stringValue);
        }
        
        public static void AddIf<T>(this IList<T> list, Func<bool> shouldAddFunc, T obj)
        {
            if (!shouldAddFunc.Invoke())
                return;
            
            list.Add(obj);
        }
        
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}