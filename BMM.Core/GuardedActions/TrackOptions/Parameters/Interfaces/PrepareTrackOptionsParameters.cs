using BMM.Api.Abstraction;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.TrackOptions.Parameters.Interfaces
{
    public interface IPrepareTrackOptionsParameters
    {
        IBaseViewModel SourceVM { get; }
        ITrackModel Track { get; }
    }
}