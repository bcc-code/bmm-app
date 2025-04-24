using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
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
using Microsoft.IdentityModel.Tokens;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class AlbumLayoutCreator : BaseLayoutCreator, IAlbumLayoutCreator
{
    private readonly IAlbumClient _albumClient;
    private readonly ITrackPOFactory _trackPOFactory;
    private readonly IMediaPlayer _mediaPlayer;
    private CPInterfaceController _cpInterfaceController;
    private int _albumId;
    private CPListTemplate _favouritesListTemplate;

    public AlbumLayoutCreator(IAlbumClient albumClient,
        ITrackPOFactory trackPOFactory,
        IMediaPlayer mediaPlayer)
    {
        _albumClient = albumClient;
        _trackPOFactory = trackPOFactory;
        _mediaPlayer = mediaPlayer;
    }

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController,
        int albumId,
        string name)
    {
        _cpInterfaceController = cpInterfaceController;
        _albumId = albumId;
        _favouritesListTemplate = new CPListTemplate(name, LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _favouritesListTemplate;
    }

    public override async Task Load()
    {
        var album = await _albumClient.GetById(_albumId);
        var trackInfoProvider = new DefaultTrackInfoProvider();

        var tracksCpListItemTemplates = await Task.WhenAll(album
            .Children
            .Select(async document =>
            {
                if (document is Track track)
                {
                    var trackPO = _trackPOFactory.Create(trackInfoProvider, null, track);

                    var coverImage = await track.ArtworkUri.ToUIImage();
                    var trackListItem = new CPListItem(trackPO.TrackTitle,
                        $"{trackPO.TrackSubtitle} {trackPO.TrackMeta}",
                        coverImage);
                    trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                    trackListItem.Handler = async (item, block) =>
                    {
                        await _mediaPlayer.Play(album.Children.OfType<IMediaTrack>().ToList(), track);
                        var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                        await CpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                        block();
                    };

                    trackListItem.AccessoryType = CPListItemAccessoryType.None;
                    return (ICPListTemplateItem)trackListItem;
                }
                else if (document is Album album)
                {
                    var coverImage = await album.Cover.ToUIImage();
                    var albumListItem = new CPListItem(album.Title, null, coverImage);
                    albumListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                    albumListItem.Handler = async (item, block) =>
                    {
                        var albumLayout = await Create(CpInterfaceController, album.Id, album.Title);
                        await CpInterfaceController.PushTemplateAsync(albumLayout, true);
                        block();
                    };

                    albumListItem.AccessoryType = CPListItemAccessoryType.None;
                    return (ICPListTemplateItem)albumListItem;
                }

                return default;
            }));

        var section = new CPListSection(tracksCpListItemTemplates);
        _favouritesListTemplate.UpdateSections(section.EncloseInArray());
    }
}