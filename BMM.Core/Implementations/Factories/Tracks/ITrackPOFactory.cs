using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Tracks;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.Tracks
{
    public interface ITrackPOFactory
    {
        TrackPO Create(
            ITrackInfoProvider trackInfoProvider,
            IMvxAsyncCommand<Document> optionsClickedCommand,
            Track track);
    }
}