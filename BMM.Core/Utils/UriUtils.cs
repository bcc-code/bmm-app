using System;

namespace BMM.Core.Utils
{
    public class UriUtils
    {
        public static bool TryCreate(string url, out Uri uri)
        {
            try
            {
                uri = new Uri(url);
                return true;
            }
            catch
            {
                Console.WriteLine($"Cannot create Uri object from: {url}");
                uri = null;
                return false;
            }
        }
    }
}