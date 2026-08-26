using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.FileStorage;

namespace BMM.Core.Implementations.Media
{
    public class MediaFileUrlSetter
    {
        private readonly IStorageManager _storageManager;
        private readonly ITrackMediaHelper _trackMediaHelper;
        private readonly IAnalytics _analytics;

        /// <summary>
        /// SetLocalPathIfDownloaded runs for every track of a queue, so a systematic mismatch would report
        /// the same track over and over. Reporting each track once per session keeps the volume to
        /// "how many distinct tracks are affected", which is the number we are after anyway.
        /// </summary>
        private readonly ConcurrentDictionary<string, byte> _alreadyReportedTracks = new();

        public MediaFileUrlSetter(IStorageManager storageManager, ITrackMediaHelper trackMediaHelper, IAnalytics analytics)
        {
            _storageManager = storageManager;
            _trackMediaHelper = trackMediaHelper;
            _analytics = analytics;
        }

        public void SetLocalPathIfDownloaded(IMediaTrack mediaFile)
        {
            if(!(mediaFile is Track track))
            {
                throw new ArgumentException("Can only set url of tracks");
            }

            var trackMediaFile = _trackMediaHelper.GetFileByMediaType(track.Media, track.MediaType);

            if (trackMediaFile != null && _storageManager.SelectedStorage.IsDownloaded(track))
            {
                mediaFile.LocalPath = _storageManager.SelectedStorage.GetUrlByFile(trackMediaFile);
                LogIfTheCheckedFileIsNotTheResolvedFile(track, mediaFile.LocalPath);
            }
            else
            {
                mediaFile.LocalPath = null;
            }
        }

        /// <summary>
        /// Diagnostic, not a fix. IsDownloaded checks the path derived from <see cref="Track.Url"/>, which
        /// is the first file of the matching media, while LocalPath is derived from the first file the
        /// platform can actually play. When those two disagree the player is handed a file that was never
        /// verified to exist, which surfaces as NSURLErrorFileDoesNotExist (-1100) during playback.
        /// This measures how often it happens and should be removed once we have the answer.
        /// </summary>
        private void LogIfTheCheckedFileIsNotTheResolvedFile(Track track, string resolvedPath)
        {
            string checkedPath = _storageManager.SelectedStorage.GetUrlByFile((IDownloadable)track);

            if (checkedPath == resolvedPath)
                return;

            if (!_alreadyReportedTracks.TryAdd(track.GetUniqueKey, 0))
                return;

            _analytics.LogEvent(
                "Downloaded track resolved to a different file than the one that was checked",
                new Dictionary<string, object>
                {
                    {"trackId", track.Id},
                    {"language", track.Language},
                    {"subtype", track.Subtype},
                    // Only the file names, so we do not ship absolute paths that carry the app container id.
                    {"checkedFile", FileNameOf(checkedPath)},
                    {"resolvedFile", FileNameOf(resolvedPath)},
                    // False means this playback is guaranteed to fail with -1100.
                    {"resolvedFileExists", resolvedPath != null && File.Exists(resolvedPath)}
                });
        }

        private static string FileNameOf(string path)
        {
            return string.IsNullOrEmpty(path)
                ? "null"
                : Path.GetFileName(path);
        }
    }
}
