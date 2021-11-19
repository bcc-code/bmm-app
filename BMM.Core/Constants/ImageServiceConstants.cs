using System;

namespace BMM.Core.Constants
{
    public class ImageServiceConstants
    {
        public const string ImageCacheFolder = "img";
        public const int ImageCacheMemorySize = 100 * 1024 * 1024; //100 MB

        public static readonly TimeSpan DiskCacheDuration = TimeSpan.FromDays(30);
    }
}