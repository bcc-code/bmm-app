using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.TrackCollections.Exceptions;

namespace BMM.Core.Implementations.TrackCollections
{
    public class TrackCollectionManager : ITrackCollectionManager
    {
        private readonly IBMMClient _bmmClient;
        private readonly IAnalytics _analytics;
        private readonly IOfflineTrackCollectionStorage _trackCollectionStorage;
        private readonly IGlobalMediaDownloader _globalMediaDownloader;
        private readonly ILogger _logger;
        private readonly IExceptionHandler _exceptionHandler;

        public TrackCollectionManager(IBMMClient bmmClient, IAnalytics analytics, IOfflineTrackCollectionStorage trackCollectionStorage,
             IGlobalMediaDownloader globalMediaDownloader, ILogger logger, IExceptionHandler exceptionHandler)
        {
            _bmmClient = bmmClient;
            _analytics = analytics;
            _trackCollectionStorage = trackCollectionStorage;
            _globalMediaDownloader = globalMediaDownloader;
            _logger = logger;
            _exceptionHandler = exceptionHandler;
        }

        public async Task DownloadTrackCollection(TrackCollection trackCollection)
        {
            await _trackCollectionStorage.Add(trackCollection.Id);
            _analytics.LogEvent("Playlist was toggled to offline content",
                new Dictionary<string, object>{
                    {"Id", trackCollection.Id},
                    {"NumberOfTracks", trackCollection.TrackCount}
                });
            await _globalMediaDownloader.SynchronizeOfflineTracks();
        }

        public async Task RemoveDownloadedTrackCollection(TrackCollection trackCollection)
        {
            await _trackCollectionStorage.Remove(trackCollection.Id);
            _analytics.LogEvent("Downloaded playlist was removed",
                new Dictionary<string, object>{
                    {"Id", trackCollection.Id},
                    {"NumberOfTracks", trackCollection.TrackCount}
                });
            await _globalMediaDownloader.SynchronizeOfflineTracks();
        }

        public bool IsOfflineAvailable(TrackCollection trackCollection)
        {
            return _trackCollectionStorage.IsOfflineAvailable(trackCollection);
        }

        public async Task AddToTrackCollection(TrackCollection trackCollection, int id, DocumentType type)
        {
            if (id == 0)
            {
                _logger.Error("TrackCollectionManager", "AddToTrackCollection called with 0 id", new NullTrackException());
                return;
            }

            if (type == DocumentType.Album)
            {
                await _bmmClient.TrackCollection.AddAlbumToTrackCollection(trackCollection.Id, id);
            }
            else if (type == DocumentType.Track)
            {
                await _bmmClient.TrackCollection.AddTracksToTrackCollection(trackCollection.Id, new List<int> {id});
            }
            else
            {
                throw new UnsupportedDocumentTypeException();
            }

            var isOfflineAvailable = _trackCollectionStorage.IsOfflineAvailable(trackCollection);
            if (isOfflineAvailable)
            {
                _exceptionHandler.FireAndForgetWithoutUserMessages(_globalMediaDownloader.SynchronizeOfflineTracks);
            }
        }
    }
}