using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Contributors;
using BMM.Core.Models.POs.Contributors.Interfaces;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.Tracks.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Recommendations;

public class RecommendationPO : DocumentPO
{
    public RecommendationPO(
        Recommendation recommendation,
        ITrackPOFactory trackPOFactory,
        ITrackInfoProvider trackInfoProvider,
        IMvxAsyncCommand<Document> optionsClickedCommand) : base(recommendation)
    {
        Recommendation = recommendation;
        
        if (recommendation.Track != null)
            TrackPO = trackPOFactory.Create(trackInfoProvider, optionsClickedCommand, recommendation.Track);
        
        if (recommendation.Contributor != null)
            ContributorPO = new ContributorPO(optionsClickedCommand, recommendation.Contributor);
        
        if (recommendation.Playlist != null)
            PlaylistPO = new PlaylistPO(recommendation.Playlist);
    }

    public bool IsDescriptionVisible => !Recommendation.Title.IsNullOrEmpty() || !Recommendation.Subtitle.IsNullOrEmpty();
    public Recommendation Recommendation { get; }
    public ITrackPO TrackPO { get; }
    public IContributorPO ContributorPO { get; }
    public PlaylistPO PlaylistPO { get; }
}