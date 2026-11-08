using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.TrackInformation.Strategies;
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
        var podcastTracks = await PodcastClient.GetTracks(_podcastId, CachePolicy.UseCacheAndWaitForUpdates);
        var trackInfoProvider = new DefaultTrackInfoProvider();
        var covers = await podcastTracks.DownloadCovers();
        
        var tracksCpListItemTemplates = new List<ICPListTemplateItem>();
        tracksCpListItemTemplates.AddIfNotNull(ShuffleButtonCreator.CreateForPodcast(_podcastId, this.CreatePlaybackOrigin(), _cpInterfaceController));;

        foreach (var track in podcastTracks)
        {
            var trackPO = TrackPOFactory.Create(trackInfoProvider, null, track);
                
            var trackListItem = new CPListItem(trackPO.TrackTitle, $"{trackPO.TrackSubtitle} {trackPO.TrackMeta}", covers.GetCover(track.ArtworkUri));
            trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

            trackListItem.Handler = async (item, block) =>
            {
                await CarPlayPlayerPresenter.PlayAndShowPlayer(
                    podcastTracks.OfType<IMediaTrack>().ToList(),
                    track,
                    this.CreatePlaybackOrigin(),
                    CpInterfaceController);
                block();
            };

            trackListItem.AccessoryType = CPListItemAccessoryType.None;
            tracksCpListItemTemplates.Add(trackListItem);
        }
        
        var section = new CPListSection(tracksCpListItemTemplates.ToArray());
        _podcastListTemplate.SafeUpdateSections(section.EncloseInArray());
    }
}