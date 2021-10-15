using System;
using Newtonsoft.Json;

namespace BMM.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Simple Copy method using serializing and deserializing JSON.
        /// </summary>
        public static T CopyBySerialization<T>(this T objectToCopy)
        {
            string serialized = JsonConvert.SerializeObject(objectToCopy);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static void IfNotNull<T>(this T obj, Action<T> action)
        {
            if (obj == null)
                return;

            action(obj);
        }
    }
}