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
using Microsoft.IdentityModel.Tokens;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class PodcastLayoutCreator : BaseLayoutCreator, IPodcastLayoutCreator
{
    private IPodcastClient PodcastClient => Mvx.IoCProvider!.Resolve<IPodcastClient>();
    private ITrackPOFactory TrackPOFactory => Mvx.IoCProvider!.Resolve<ITrackPOFactory>();
    private IMediaPlayer MediaPlayer => Mvx.IoCProvider!.Resolve<IMediaPlayer>();
    private int _podcastId;
    private CPListTemplate _podcastListTemplate;
    private CPInterfaceController _cpInterfaceController;

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(
        CPInterfaceController cpInterfaceController,
        int podcastId,
        string name)
    {
        _cpInterfaceController = cpInterfaceController;
        _podcastId = podcastId;
        _podcastListTemplate = new CPListTemplate(name, LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _podcastListTemplate;
    }

    public override async Task Load()
    {
        var podcastTracks = await PodcastClient.GetTracks(_podcastId, CachePolicy.UseCacheAndRefreshOutdated);
        var trackInfoProvider = new DefaultTrackInfoProvider();

        var tracksCpListItemTemplates = await Task.WhenAll(podcastTracks
            .Select(async track =>
            {
                var trackPO = TrackPOFactory.Create(trackInfoProvider, null, track);
                
                var coverImage = await track.ArtworkUri.ToUIImage();
                var trackListItem = new CPListItem(trackPO.TrackTitle, $"{trackPO.TrackSubtitle} {trackPO.TrackMeta}", coverImage);
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                trackListItem.Handler = async (item, block) =>
                {
                    await MediaPlayer.Play(podcastTracks.OfType<IMediaTrack>().ToList(), track, this.CreatePlaybackOrigin());
                    var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                    await CpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracksCpListItemTemplates);
        _podcastListTemplate.SafeUpdateSections(section.EncloseInArray());
    }
}