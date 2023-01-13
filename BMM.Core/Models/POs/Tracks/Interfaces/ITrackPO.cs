using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.Tracks.Interfaces
{
    public interface ITrackPO : IDocumentPO, ITrackHolderPO
    {
        Track Track { get; }
        string TrackTitle { get; }
        string TrackSubtitle { get; }
        string TrackMeta { get; }
        TrackState TrackState { get; }
    }
}