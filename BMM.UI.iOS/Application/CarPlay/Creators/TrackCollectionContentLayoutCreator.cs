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
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class TrackCollectionContentLayoutCreator : BaseLayoutCreator, ITrackCollectionContentLayoutCreator
{
    private ITrackCollectionClient TrackCollectionClient => Mvx.IoCProvider!.Resolve<ITrackCollectionClient>();
    private ITrackPOFactory TrackPOFactory => Mvx.IoCProvider!.Resolve<ITrackPOFactory>();
    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _trackCollectionListTemplate;
    private int _trackCollectionId;

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
        var trackCollection = await TrackCollectionClient.GetById(_trackCollectionId, CachePolicy.UseCacheAndRefreshOutdated);

        var tracks = trackCollection
            .Tracks
            .Select(t => TrackPOFactory.Create(trackInfoProvider, null, t))
            .ToList();

        var covers = await tracks.DownloadCovers();

        var tracksCpListItemTemplates = await Task.WhenAll(tracks
            .Select(async x =>
            {
                var trackListItem = new CPListItem(x.TrackTitle, $"{x.TrackSubtitle} {x.TrackMeta}", covers.GetCover(x.Track.ArtworkUri));

                trackListItem.Handler = async (item, block) =>
                {
                    await CarPlayPlayerPresenter.PlayAndShowPlayer(
                        trackCollection.Tracks.OfType<IMediaTrack>().ToList(),
                        x.Track,
                        this.CreatePlaybackOrigin(),
                        CpInterfaceController);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracksCpListItemTemplates);
        _trackCollectionListTemplate.SafeUpdateSections(section.EncloseInArray());
    }
}