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

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class TrackCollectionContentLayoutCreator : BaseLayoutCreator, ITrackCollectionContentLayoutCreator
{
    private readonly ITrackCollectionClient _trackCollectionClient;
    private readonly ITrackPOFactory _trackPOFactory;
    private readonly IMediaPlayer _mediaPlayer;
    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _trackCollectionListTemplate;
    private int _trackCollectionId;

    public TrackCollectionContentLayoutCreator(
        ITrackCollectionClient trackCollectionClient,
        ITrackPOFactory trackPOFactory,
        IMediaPlayer mediaPlayer)
    {
        _trackCollectionClient = trackCollectionClient;
        _trackPOFactory = trackPOFactory;
        _mediaPlayer = mediaPlayer;
    }

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, string title, int trackCollectionId)
    {
        _cpInterfaceController = cpInterfaceController;
        _trackCollectionId = trackCollectionId;
        _trackCollectionListTemplate = new CPListTemplate(title, LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _trackCollectionListTemplate;
    }

    public override async Task Load()
    {
        var trackInfoProvider = new DefaultTrackInfoProvider();
        var trackCollection = await _trackCollectionClient.GetById(_trackCollectionId, CachePolicy.UseCacheAndRefreshOutdated);

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
                    await CpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracksCpListItemTemplates);
        _trackCollectionListTemplate.UpdateSections(section.EncloseInArray());
    }
}