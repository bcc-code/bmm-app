using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Tracks.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.HighlightedTextTracks
{
    public interface IHighlightedTextTrackPOFactory
    {
        IHighlightedTextTrackPO Create(
            HighlightedTextTrack highlightedTextTrack,
            IMvxAsyncCommand<Document> optionsClickedCommand,
            ITrackInfoProvider trackInfoProvider);
    }
}