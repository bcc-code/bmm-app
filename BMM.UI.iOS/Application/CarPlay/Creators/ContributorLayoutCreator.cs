using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.CarPlay.Creators.Base;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class ContributorLayoutCreator : BaseLayoutCreator, IContributorLayoutCreator
{
    private readonly IContributorClient _contributorClient;
    private readonly ITrackPOFactory _trackPOFactory;
    private readonly IMediaPlayer _mediaPlayer;
    private CPInterfaceController _cpInterfaceController;
    private int _contributorId;
    private CPListTemplate _favouritesListTemplate;

    public ContributorLayoutCreator(
        IContributorClient contributorClient,
        ITrackPOFactory trackPOFactory,
        IMediaPlayer mediaPlayer)
    {
        _contributorClient = contributorClient;
        _trackPOFactory = trackPOFactory;
        _mediaPlayer = mediaPlayer;
    }

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int contributorId, string name)
    {
        _cpInterfaceController = cpInterfaceController;
        _contributorId = contributorId;
        _favouritesListTemplate = new CPListTemplate(name, LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _favouritesListTemplate;
    }

    public override async Task Load()
    {
        var tracks = await _contributorClient.GetTracks(_contributorId, CachePolicy.UseCacheAndRefreshOutdated);
        var trackInfoProvider = new DefaultTrackInfoProvider();
        
        var tracksPOs = tracks
            .Select(t => _trackPOFactory.Create(trackInfoProvider, null, t))
            .ToList();
        
        var tracksCpListItemTemplates = await Task.WhenAll(tracksPOs
            .Select(async x =>
            {
                var coverImage = await x.Track.ArtworkUri.ToUIImage();
                var trackListItem = new CPListItem(x.TrackTitle, $"{x.TrackSubtitle} {x.TrackMeta}", coverImage);
               
                trackListItem.Handler = async (item, block) =>
                {
                    await _mediaPlayer.Play(tracks.OfType<IMediaTrack>().ToList(), x.Track);
                    var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                    await CpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));
        
        var section = new CPListSection(tracksCpListItemTemplates);
        _favouritesListTemplate.UpdateSections(section.EncloseInArray());
    }
}