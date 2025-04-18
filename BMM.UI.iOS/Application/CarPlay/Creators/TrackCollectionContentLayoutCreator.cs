using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
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

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, string title, int trackCollectionId)
    {
        var trackCollectionListTemplate = new CPListTemplate(title, LoadingSection.Create());
        Load(cpInterfaceController, trackCollectionListTemplate, trackCollectionId).FireAndForget();
        return trackCollectionListTemplate;
    }

    private async Task Load(CPInterfaceController cpInterfaceController, CPListTemplate trackCollectionListTemplate, int trackCollectionId)
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
                var coverImage = await x.Track.ArtworkUri.ToUIImage();
                var trackListItem = new CPListItem(x.TrackTitle, $"{x.TrackSubtitle} {x.TrackMeta}", coverImage);

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
        trackCollectionListTemplate.UpdateSections(section.EncloseInArray());
    }
}