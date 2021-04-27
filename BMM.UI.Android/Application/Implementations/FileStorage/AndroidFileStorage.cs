using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.FileStorage;
using MvvmCross;

namespace BMM.UI.Droid.Application.Implementations.FileStorage
{
    public class AndroidFileStorage : IFileStorage
    {
        private readonly Java.IO.File _filesDir;
        private readonly IAnalytics _analytics;

        public AndroidFileStorage(Java.IO.File fileDir, StorageKind storageKind, IAnalytics analytics)
        {
            StorageKind = storageKind;
            _filesDir = fileDir;
            _analytics = analytics;
        }

        public string Path => _filesDir.Path;

        public StorageKind StorageKind { get; }

        public long TotalSpace => _filesDir.TotalSpace;

        public long UsableSpace => _filesDir.UsableSpace;

        public long FreeSpace => _filesDir.FreeSpace;

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
            var isDownloading = Mvx.IoCProvider.Resolve<IDownloadQueue>().IsDownloading(downloadable);

            return File.Exists(path) && !isDownloading;
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
            return (int)Math.Floor((decimal)_filesDir.FreeSpace * 100 / TotalSpace);
        }

        public void DeleteFileByUrl(string url)
        {
            var path = PathByUrl(url);
            File.Delete(path);
            _analytics.LogEvent("deleted file", new Dictionary<string, object> {{"url", url}, {"path", path}});
        }

        private string PathByUrl(string url)
        {
            string fileName = Android.Net.Uri.Parse(url).Path.Split('/').Last();

            return System.IO.Path.Combine(Path, fileName);
        }
    }
}