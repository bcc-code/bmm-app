using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Device;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class AVPlayerItemRepository : IAVPlayerItemRepository
    {
        private readonly IAVPlayerItemFactory _avPlayerItemFactory;
        private readonly IMediaRequestHttpHeaders _mediaRequestHttpHeaders;
        private readonly IFeatureSupportInfoService _featureSupportInfoService;
        private readonly ConcurrentDictionary<string, CacheAVPlayerItemLoader> _loaders = new ConcurrentDictionary<string, CacheAVPlayerItemLoader>();
        private string _currentFileInUse;

        public AVPlayerItemRepository(
            IAVPlayerItemFactory avPlayerItemFactory,
            IMediaRequestHttpHeaders mediaRequestHttpHeaders,
            IFeatureSupportInfoService featureSupportInfoService)
        {
            _avPlayerItemFactory = avPlayerItemFactory;
            _mediaRequestHttpHeaders = mediaRequestHttpHeaders;
            _featureSupportInfoService = featureSupportInfoService;
        }
        
        public async Task AddAndLoad(IMediaTrack mediaTrack)
        {
            bool shouldSkip = !_featureSupportInfoService.SupportsAVPlayerItemCache
                            || _loaders.ContainsKey(mediaTrack.GetUniqueKey)
                            || mediaTrack.IsDownloaded()
                            || TryGetCachedFileNameIfAvailableAndValid(mediaTrack.GetUniqueKey, out _)
                            || mediaTrack.TrackMediaFile.Size > CacheMediaFileHandle.MaximumSingleFileSize;
            
            if (shouldSkip)
                return;

            CancelPreviousDownloadingIfNeeded();
            PrepareSpaceForFile(mediaTrack.TrackMediaFile.Size);
            var cacheAVPlayerItemLoader = new CacheAVPlayerItemLoader(_mediaRequestHttpHeaders, mediaTrack.GetUniqueKey);
            cacheAVPlayerItemLoader.FinishedLoading += CacheAVPlayerItemLoaderOnFinishedLoading;
            await cacheAVPlayerItemLoader.StartDataRequest(mediaTrack.Url);
            _loaders.TryAdd(mediaTrack.GetUniqueKey, cacheAVPlayerItemLoader);
        }
        
        public async Task<AVPlayerItem> Get(IMediaTrack mediaTrack)
        {
            if (_featureSupportInfoService.SupportsAVPlayerItemCache && TryGetCachedFileNameIfAvailableAndValid(mediaTrack.GetUniqueKey, out string fileName))
            {
                _currentFileInUse = Path.Combine(CacheMediaFileHandle.AVPlayerItemsCacheDirectoryPath, fileName);
                return _avPlayerItemFactory.Create(_currentFileInUse);
            }

            _currentFileInUse = null;
            return await _avPlayerItemFactory.Create(mediaTrack);
        }

        public void SynchronizeCacheFiles()
        {
            if (!_featureSupportInfoService.SupportsAVPlayerItemCache)
                return;
            
            var cacheFiles = GetAllCachedFiles(false);

            foreach (string cacheFile in cacheFiles)
            {
                if (!cacheFile.Contains(CacheMediaFileHandle.LoadingIndicator))
                    continue;
                
                var file = CacheMediaFileHandle.OpenExistingFile(Path.Combine(CacheMediaFileHandle.AVPlayerItemsCacheDirectoryPath, cacheFile));
                file.DeleteFile();
                file.Close();
            }
        }
        
        public IList<string> GetAllCachedFiles(bool onlyFullyLoaded)
        {
            string[] allFiles = NSFileManager
                .DefaultManager
                .GetDirectoryContent(CacheMediaFileHandle.AVPlayerItemsCacheDirectoryPath, out _);

            string[] cacheFiles =  allFiles
                .Where(f => f.Contains(CacheMediaFileHandle.BMMCachePrefix))
                .ToArray();

            if (!onlyFullyLoaded)
                return cacheFiles;

            return cacheFiles
                .Where(f => !f.Contains(CacheMediaFileHandle.LoadingIndicator))
                .ToArray();
        }

        private void CancelPreviousDownloadingIfNeeded()
        {
            var loaders = _loaders
                .Values
                .Where(x => !x.MoreThanAHalfFinished);

            foreach (var loader in loaders)
            {
                loader.Cancel();
                loader.FinishedLoading -= CacheAVPlayerItemLoaderOnFinishedLoading;
                _loaders.TryRemove(loader.UniqueKey, out _);
            }
        }

        private void CacheAVPlayerItemLoaderOnFinishedLoading(object sender, bool endedWithError)
        {
            string uniqueKey = ((CacheAVPlayerItemLoader)sender).UniqueKey;
            
            if (!_loaders.TryGetValue(uniqueKey, out var loader))
                return;
            
            loader.FinishedLoading -= CacheAVPlayerItemLoaderOnFinishedLoading;
            _loaders.TryRemove(uniqueKey, out _);
        }

        private bool TryGetCachedFileNameIfAvailableAndValid(string uniqueKey, out string path)
        {
            path = null;
            
            var allFiles = GetAllCachedFiles(true);
            string cacheFile = allFiles
                .FirstOrDefault(f => f.Contains(uniqueKey));

            if (cacheFile == null)
                return false;

            path = cacheFile;
            return true;
        }

        private void PrepareSpaceForFile(long size)
        {
            var allFiles = GetAllCachedFiles(true);

            var filesWithSize = allFiles
                .Select(f => (FileName: f, Size: f.GetCachePlayerItemExpectedSize()))
                .ToList();
            
            long allFilesSize = filesWithSize
                .Sum(f => f.Size);
            
            if (allFilesSize + size <= CacheMediaFileHandle.MaximumSpaceForCache)
                return;

            long memoryToRelease = CacheMediaFileHandle.MaximumSpaceForCache - allFilesSize + size;
            var orderedFiles = filesWithSize
                .OrderBy(f => f.FileName.GetCachePlayerItemCreatedTime());

            long currentMemoryReleased = 0;
            
            foreach (var file in orderedFiles)
            {
                if (file.FileName == _currentFileInUse)
                    continue;
                
                currentMemoryReleased += file.Size;
                NSFileManager.DefaultManager.Remove(Path.Combine(CacheMediaFileHandle.AVPlayerItemsCacheDirectoryPath, file.FileName), out _);
                
                if (currentMemoryReleased >= memoryToRelease)
                    break;
            }
        }
    }
}