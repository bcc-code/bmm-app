using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.Tracks.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.Tracks
{
    public interface ITrackPOFactory
    {
        ITrackPO Create(
            ITrackInfoProvider trackInfoProvider,
            IMvxAsyncCommand<Document> optionsClickedCommand,
            Track track);
    }
}