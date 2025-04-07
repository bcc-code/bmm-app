using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using CarPlay;
using FFImageLoading;

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
        var playlistTracks = await _playlistClient.GetTracks(playlistId, CachePolicy.UseCacheAndRefreshOutdated);
        var audiobookPodcastInfoProvider = new AudiobookPodcastInfoProvider(new DefaultTrackInfoProvider());

        var tracksCpListItemTemplates = await Task.WhenAll(playlistTracks
            .Select(async track =>
            {
                var trackPO = _trackPOFactory.Create(audiobookPodcastInfoProvider, null, track);
                
                var coverImage = await ImageService
                    .Instance
                    .LoadUrl(track.ArtworkUri)
                    .AsUIImageAsync();

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
        var favouritesListTemplate = new CPListTemplate(name, section.EncloseInArray());
        return favouritesListTemplate;
    }
}