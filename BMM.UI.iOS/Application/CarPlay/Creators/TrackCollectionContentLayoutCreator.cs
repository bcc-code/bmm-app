using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class TrackCollectionContentLayoutCreator : ITrackCollectionContentLayoutCreator
{
    private readonly ITrackCollectionClient _trackCollectionClient;
    private readonly ITrackPOFactory _trackPOFactory;
    private readonly IMediaPlayer _mediaPlayer;

    public TrackCollectionContentLayoutCreator(
        ITrackCollectionClient trackCollectionClient,
        ITrackPOFactory trackPOFactory,
        IMediaPlayer mediaPlayer)
    {
        _trackCollectionClient = trackCollectionClient;
        _trackPOFactory = trackPOFactory;
        _mediaPlayer = mediaPlayer;
    }

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int trackCollectionId)
    {
        var trackInfoProvider = new DefaultTrackInfoProvider();
        var trackCollection = await _trackCollectionClient.GetById(trackCollectionId, CachePolicy.UseCacheAndRefreshOutdated);

        var tracks = trackCollection
            .Tracks
            .Select(t => _trackPOFactory.Create(trackInfoProvider, null, t))
            .ToList();

        var tracksCpListItemTemplates = await Task.WhenAll(tracks
            .Select(async x =>
            {
                var coverImage = await ImageService
                    .Instance
                    .LoadUrl(x.Track.ArtworkUri)
                    .AsUIImageAsync();

                var trackListItem = new CPListItem(x.TrackTitle, $"{x.TrackSubtitle} {x.TrackMeta}", coverImage);
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                trackListItem.Handler = async (item, block) =>
                {
                    await _mediaPlayer.Play(trackCollection.Tracks.OfType<IMediaTrack>().ToList(), x.Track);
                    var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                    await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracksCpListItemTemplates);
        var favouritesListTemplate = new CPListTemplate(trackCollection.Name, section.EncloseInArray());
        return favouritesListTemplate;
    }
}