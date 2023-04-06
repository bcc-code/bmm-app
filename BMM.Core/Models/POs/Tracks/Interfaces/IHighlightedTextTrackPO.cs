using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.UI.StyledText;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tracks.Interfaces;

public interface IHighlightedTextTrackPO : IDocumentPO
{
    HighlightedTextTrack HighlightedTextTrack { get; } 
    ITrackPO TrackPO { get; } 
    IMvxAsyncCommand ItemClickedCommand { get; }
    StyledTextContainer StyledTextContainer { get; }
    float RatioOfFirstHighlightPositionToFullText { get; }
}