using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.TrackCollections;

namespace BMM.Core.Implementations.Factories.TrackCollections
{
    public interface ITrackCollectionPOFactory
    {
        TrackCollectionPO Create(TrackCollection trackCollection);
    }
}