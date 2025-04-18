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
public class PlaylistLayoutCreator : IPlaylistLayoutCreator
{
    private readonly IPlaylistClient _playlistClient;
    private readonly ITrackPOFactory _trackPOFactory;
    private readonly IMediaPlayer _mediaPlayer;

    public PlaylistLayoutCreator(
        IPlaylistClient playlistClient,
        ITrackPOFactory trackPOFactory,
        IMediaPlayer mediaPlayer)
    {
        _playlistClient = playlistClient;
        _trackPOFactory = trackPOFactory;
        _mediaPlayer = mediaPlayer;
    }
    
    public async Task<CPListTemplate> Create(
        CPInterfaceController cpInterfaceController,
        int playlistId,
        string name)
    {
        var playlistListTemplate = new CPListTemplate(name, LoadingSection.Create());
        Load(cpInterfaceController, playlistListTemplate, playlistId).FireAndForget();
        return playlistListTemplate;
    }

    private async Task Load(
        CPInterfaceController cpInterfaceController,
        CPListTemplate playlistListTemplate,
        int playlistId)
    {
        var playlistTracks = await _playlistClient.GetTracks(playlistId, CachePolicy.UseCacheAndRefreshOutdated);
        var audiobookPodcastInfoProvider = new AudiobookPodcastInfoProvider(new DefaultTrackInfoProvider());

        var tracksCpListItemTemplates = await Task.WhenAll(playlistTracks
            .Select(async track =>
            {
                var trackPO = _trackPOFactory.Create(audiobookPodcastInfoProvider, null, track);
                
                var coverImage = await track.ArtworkUri.ToUIImage();
                var trackListItem = new CPListItem(trackPO.TrackTitle, $"{trackPO.TrackSubtitle} {trackPO.TrackMeta}", coverImage);
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                trackListItem.Handler = async (item, block) =>
                {
                    await _mediaPlayer.Play(playlistTracks.OfType<IMediaTrack>().ToList(), track);
                    var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                    await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracksCpListItemTemplates);
        playlistListTemplate.UpdateSections(section.EncloseInArray());
    }
}