using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.DownloadManager;

namespace BMM.Core.Implementations.FileStorage
{
    public interface IFileStorage
    {
        StorageKind StorageKind { get; }

        long TotalSpace { get; }

        long UsableSpace { get; }

        long FreeSpace { get; }

        List<string> IdsOfDownloadedFiles();

        int RemoveAllDownloadedFilesAndGetCount();

        List<string> PathsOfDownloadedFiles();

        bool IsDownloaded(IDownloadable downloadable);

        void DeleteFileByUrl(string url);

        void DeleteFile(IDownloadable file);

        string GetUrlByFile(TrackMediaFile file);

        string GetUrlByFile(IDownloadFile file);

        string GetUrlByFile(IDownloadable downloadable);

        int FreeSpacePercent();
    }
}