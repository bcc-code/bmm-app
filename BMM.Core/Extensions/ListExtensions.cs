using System;
using System.Collections.Generic;

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
    }
}