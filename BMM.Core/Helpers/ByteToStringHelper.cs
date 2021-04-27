using System;

namespace BMM.Core.Helpers
{
    public static class ByteToStringHelper
    {
        public static string BytesToString(long bytes, out bool showMbMarker)
        {
            showMbMarker = false;
            var convertedBytes = BytesToGigaBytes(bytes);
            if (convertedBytes < 0.9)
            {
                showMbMarker = true;
                convertedBytes = BytesToMegaBytes(bytes);
                return convertedBytes.ToString();
            }

            return convertedBytes.ToString("N2");
        }

        public static double BytesToMegaBytes(long bytes)
        {
            return Math.Floor(bytes / Math.Pow(1024, 2));
        }

        private static double BytesToGigaBytes(long bytes)
        {
            return bytes / Math.Pow(1024, 3);
        }
    }
}
