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
    }
}