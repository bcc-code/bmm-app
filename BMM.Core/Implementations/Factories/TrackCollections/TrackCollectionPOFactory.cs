using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Models.POs.TrackCollections;

namespace BMM.Core.Implementations.Factories.TrackCollections
{
    public class TrackCollectionPOFactory : ITrackCollectionPOFactory
    {
        private readonly IOfflineTrackCollectionStorage _offlineTrackCollectionStorage;

        public TrackCollectionPOFactory(IOfflineTrackCollectionStorage offlineTrackCollectionStorage)
        {
            _offlineTrackCollectionStorage = offlineTrackCollectionStorage;
        }
        
        public TrackCollectionPO Create(TrackCollection trackCollection)
        {
            return new TrackCollectionPO(_offlineTrackCollectionStorage, trackCollection);
        }
    }
}