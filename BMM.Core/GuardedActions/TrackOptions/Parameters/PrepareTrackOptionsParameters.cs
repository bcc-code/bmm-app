using BMM.Api.Abstraction;
using BMM.Core.GuardedActions.TrackOptions.Parameters.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.TrackOptions.Parameters
{
    public class PrepareTrackOptionsParameters : IPrepareTrackOptionsParameters
    {
        public PrepareTrackOptionsParameters(IBaseViewModel sourceVM, ITrackModel track)
        {
            SourceVM = sourceVM;
            Track = track;
        }

        public IBaseViewModel SourceVM { get; }
        public ITrackModel Track { get; }
    }
}