using System;
using System.IO;
using BMM.Core.Constants;
using BMM.UI.iOS.Extensions;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class CacheMediaFileHandle : IDisposable
    {
        public const long MaximumSingleFileSize = 104857600; // 100 MB
        public const long MaximumSpaceForCache = 157286400; // 150 MB
        public const string BMMCachePrefix = "bmm_cache";
        public const string Extension = ".mp3";
        
        // INPR: In progress - means that file is not fully filled with data
        public const string LoadingIndicator = "INPR_";
        
        private NSFileHandle _writeHandle;
        private readonly object _lock = new object();

        public static CacheMediaFileHandle CreateNewFile(string uniqueKey, long expectedContentLength)
        {
            return new CacheMediaFileHandle(uniqueKey, expectedContentLength);
        }
        
        public static CacheMediaFileHandle OpenExistingFile(string filePath)
        {
            return new CacheMediaFileHandle(filePath);
        }

        private CacheMediaFileHandle(string uniqueKey, long expectedContentLength)
        {
            FilePath = GetFilePath(uniqueKey, expectedContentLength);
            
            if (NSFileManager.DefaultManager.FileExists(FilePath))
                DeleteFile();

            NSFileManager.DefaultManager.CreateFile(FilePath, new NSData(), attr: null);
            Init();
        }
        
        private CacheMediaFileHandle(string filePath)
        {
            FilePath = filePath;
        }
        
        public string FilePath { get; }

        public static string AVPlayerItemsCacheDirectoryPath => NSFileManager
            .DefaultManager
            .GetUrl(NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User, null, true, out _)
            .Path;
        
        public void Dispose()
        {
            if (!NSFileManager.DefaultManager.FileExists(FilePath))
                return;

            Close();
        }

        public NSFileAttributes GetAttributes()
        {
            try
            {
                return NSFileManager.DefaultManager.GetAttributes(FilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
        
        public long GetFileSize()
        {
            ulong? size = GetAttributes()?.Size;
            
            if (size == null)
                return NumericConstants.Zero;
            
            return (long)size;
        }

        public void Append(NSData data)
        {
            lock (_lock)
            {
                _writeHandle.SeekToEndOfFile();
                _writeHandle.Write(data, out var error);
            }
        }

        private void Init()
        {
            _writeHandle = NSFileHandle.OpenWrite(FilePath);
        }
        
        private static string GetFilePath(string uniqueKey, long expectedContentLength)
        {
            return Path.Combine(AVPlayerItemsCacheDirectoryPath, $"{LoadingIndicator}{BMMCachePrefix}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{uniqueKey}_{expectedContentLength}{Extension}");
        }
        
        public void Close()
        {
            lock (_lock)
                _writeHandle?.Close(out _);
        }

        public void DeleteFile()
        {
            lock (_lock)
                NSFileManager.DefaultManager.Remove(FilePath, out var error);
        }

        public void RemoveLoadingIndicator()
        {
            string newName = FilePath
                .Replace(LoadingIndicator, string.Empty);
            
            NSFileManager.DefaultManager.Move(FilePath, newName, out _);
        }
        
        public bool HasValidSize()
        {
            return GetFileSize() == FilePath.GetCachePlayerItemExpectedSize();
        }
    }
}