using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Implementations.Factories.HighlightedTextTracks;

public class HighlightedTextTrackPOFactory : IHighlightedTextTrackPOFactory
{
    private readonly ITrackPOFactory _trackPOFactory;
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IShareLink _shareLink;

    public HighlightedTextTrackPOFactory(
        ITrackPOFactory trackPOFactory,
        IMvxNavigationService mvxNavigationService,
        IMediaPlayer mediaPlayer,
        IShareLink shareLink)
    {
        _trackPOFactory = trackPOFactory;
        _mvxNavigationService = mvxNavigationService;
        _mediaPlayer = mediaPlayer;
        _shareLink = shareLink;
    }
    
    public IHighlightedTextTrackPO Create(
        HighlightedTextTrack highlightedTextTrack,
        IMvxAsyncCommand<Document> optionsClickedCommand,
        ITrackInfoProvider trackInfoProvider)
    {
        return new HighlightedTextTrackPO(
            highlightedTextTrack,
            _trackPOFactory.Create(trackInfoProvider, optionsClickedCommand, highlightedTextTrack.Track),
            _mvxNavigationService,
            _mediaPlayer,
            _shareLink);
    }
}