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
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class AlbumLayoutCreator : BaseLayoutCreator, IAlbumLayoutCreator
{
    private IAlbumClient AlbumClient => Mvx.IoCProvider!.Resolve<IAlbumClient>();
    private ITrackPOFactory TrackPOFactory => Mvx.IoCProvider!.Resolve<ITrackPOFactory>();
    private IMediaPlayer MediaPlayer => Mvx.IoCProvider!.Resolve<IMediaPlayer>();
    private CPInterfaceController _cpInterfaceController;
    private int _albumId;
    private CPListTemplate _favouritesListTemplate;

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
        var album = await AlbumClient.GetById(_albumId);
        var trackInfoProvider = new DefaultTrackInfoProvider();

        var tracksCpListItemTemplates = await Task.WhenAll(album
            .Children
            .Select(async document =>
            {
                if (document is Track track)
                {
                    var trackPO = TrackPOFactory.Create(trackInfoProvider, null, track);

                    var coverImage = await track.ArtworkUri.ToUIImage();
                    var trackListItem = new CPListItem(trackPO.TrackTitle,
                        $"{trackPO.TrackSubtitle} {trackPO.TrackMeta}",
                        coverImage);
                    trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                    trackListItem.Handler = async (item, block) =>
                    {
                        await MediaPlayer.Play(
                            album.Children.OfType<IMediaTrack>().ToList(),
                            track,
                            this.CreatePlaybackOrigin());
                        var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                        await CpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                        block();
                    };

                    trackListItem.AccessoryType = CPListItemAccessoryType.None;
                    return trackListItem;
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