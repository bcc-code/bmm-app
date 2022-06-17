using System.Collections.Generic;

namespace BMM.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddIfNew(this IDictionary<string, object> parameters, string key, object value)
        {
            if (!parameters.ContainsKey(key))
                parameters.Add(key, value);
        }
    }
}