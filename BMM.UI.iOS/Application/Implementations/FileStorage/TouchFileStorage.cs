using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.FileStorage;
using Foundation;

namespace BMM.UI.iOS
{
    public class TouchFileStorage : IFileStorage
    {
        private readonly IAnalytics _analytics;

        public TouchFileStorage(IAnalytics analytics)
        {
            _analytics = analytics;
        }

        protected virtual string Path => NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;

        public StorageKind StorageKind => StorageKind.Internal;

        public long TotalSpace => (long)NSFileManager.DefaultManager.GetFileSystemAttributes(Path).Size;

        public long UsableSpace => (long)NSFileManager.DefaultManager.GetFileSystemAttributes(Path).FreeSize;

        public long FreeSpace => (long)NSFileManager.DefaultManager.GetFileSystemAttributes(Path).FreeSize;

        public List<string> IdsOfDownloadedFiles()
        {
            var files = PathsOfDownloadedFiles();
            List<string> downloadedFiles = files.Select(f => f
                    .Replace("track_", "")
                    .Replace("_media", "")
                    .Replace(".mp3", ""))
                .ToList();
            return downloadedFiles;
        }

        public List<string> PathsOfDownloadedFiles()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);
            return directoryInfo
                .GetFiles("*.mp3")
                .Select(x => x.Name)
                .Select(PathByUrl)
                .ToList();
        }

        public int RemoveAllDownloadedFilesAndGetCount()
        {
            var existingTracks = IdsOfDownloadedFiles();
            foreach (var existingTrack in existingTracks)
            {
                DeleteFileByUrl(existingTrack);
            }

            return existingTracks.Count;
        }

        public bool IsDownloaded(IDownloadable downloadable)
        {
            var path = PathByUrl(downloadable.Url);
            return File.Exists(path);
        }

        public void DeleteFile(IDownloadable file)
        {
            DeleteFileByUrl(file.Url);
        }

        public string GetUrlByFile(TrackMediaFile file)
        {
            return PathByUrl(file.Url);
        }

        public string GetUrlByFile(IDownloadFile file)
        {
            return PathByUrl(file.Url);
        }

        public string GetUrlByFile(IDownloadable downloadable)
        {
            return PathByUrl(downloadable.Url);
        }

        public int FreeSpacePercent()
        {
            return (int)Math.Floor((decimal)FreeSpace * 100 / TotalSpace);
        }

        public void DeleteFileByUrl(string url)
        {
            var path = PathByUrl(url);
            File.Delete(path);
            _analytics.LogEvent("deleted file", new Dictionary<string, object> {{"url", url}, {"path", path}});
        }

        private string PathByUrl(string url)
        {
            var lastPathComponent = new NSUrl(url).LastPathComponent;
            return System.IO.Path.Combine(Path, lastPathComponent);
        }
    }
}