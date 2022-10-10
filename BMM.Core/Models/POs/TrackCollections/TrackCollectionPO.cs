using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.TrackCollections
{
    public class TrackCollectionPO : DocumentPO
    {
        private readonly IOfflineTrackCollectionStorage _offlineTrackCollectionStorage;

        public TrackCollectionPO(
            IOfflineTrackCollectionStorage offlineTrackCollectionStorage,
            TrackCollection trackCollection) : base(trackCollection)
        {
            _offlineTrackCollectionStorage = offlineTrackCollectionStorage;
            TrackCollection = trackCollection;
        }
        
        public TrackCollection TrackCollection { get; }
        public bool IsAvailableOffline => _offlineTrackCollectionStorage.IsOfflineAvailable(TrackCollection);
    }
}