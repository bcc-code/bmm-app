using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;
using Microsoft.IdentityModel.Tokens;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class PodcastLayoutCreator : IPodcastLayoutCreator
{
    private readonly ITrackPOFactory _trackPOFactory;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IPodcastClient _podcastClient;

    public PodcastLayoutCreator(
        IPodcastClient podcastClient,
        ITrackPOFactory trackPOFactory,
        IMediaPlayer mediaPlayer)
    {
        _podcastClient = podcastClient;
        _trackPOFactory = trackPOFactory;
        _mediaPlayer = mediaPlayer;
    }
    
    public async Task<CPListTemplate> Create(
        CPInterfaceController cpInterfaceController,
        int podcastId,
        string name)
    {
        var podcastListTemplate = new CPListTemplate(name, LoadingSection.Create());
        Load(cpInterfaceController, podcastListTemplate, podcastId).FireAndForget();
        return podcastListTemplate;
    }

    private async Task Load(
        CPInterfaceController cpInterfaceController,
        CPListTemplate podcastListTemplate,
        int podcastId)
    {
        var podcastTracks = await _podcastClient.GetTracks(podcastId, CachePolicy.UseCacheAndRefreshOutdated);
        var trackInfoProvider = new DefaultTrackInfoProvider();

        var tracksCpListItemTemplates = await Task.WhenAll(podcastTracks
            .Select(async track =>
            {
                var trackPO = _trackPOFactory.Create(trackInfoProvider, null, track);
                
                var coverImage = await track.ArtworkUri.ToUIImage();
                var trackListItem = new CPListItem(trackPO.TrackTitle, $"{trackPO.TrackSubtitle} {trackPO.TrackMeta}", coverImage);
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                trackListItem.Handler = async (item, block) =>
                {
                    await _mediaPlayer.Play(podcastTracks.OfType<IMediaTrack>().ToList(), track);
                    var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                    await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracksCpListItemTemplates);
        podcastListTemplate.UpdateSections(section.EncloseInArray());
    }
}