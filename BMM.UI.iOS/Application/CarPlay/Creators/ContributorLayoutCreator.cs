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
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class ContributorLayoutCreator : BaseLayoutCreator, IContributorLayoutCreator
{
    private CPInterfaceController _cpInterfaceController;
    private int _contributorId;
    private CPListTemplate _favouritesListTemplate;
    private IContributorClient ContributorClient => Mvx.IoCProvider!.Resolve<IContributorClient>();
    private ITrackPOFactory TrackPOFactory => Mvx.IoCProvider!.Resolve<ITrackPOFactory>();

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int contributorId, string name)
    {
        _cpInterfaceController = cpInterfaceController;
        _contributorId = contributorId;
        _favouritesListTemplate = new CPListTemplate(name, LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _favouritesListTemplate;
    }

    public override async Task Load()
    {
        var tracks = await ContributorClient.GetTracks(_contributorId, CachePolicy.UseCacheAndWaitForUpdates);
        var trackInfoProvider = new DefaultTrackInfoProvider();

        var covers = await tracks.DownloadCovers();
        
        var tracksPOs = tracks
            .Select(t => TrackPOFactory.Create(trackInfoProvider, null, t))
            .ToList();
        
        var tracksCpListItemTemplates = new List<ICPListTemplateItem>();
        tracksCpListItemTemplates.AddIfNotNull(ShuffleButtonCreator.CreateForContributor(_contributorId, this.CreatePlaybackOrigin(), _cpInterfaceController));

        tracksPOs.ForEach(x =>
            {
                var trackListItem = new CPListItem(x.TrackTitle, $"{x.TrackSubtitle} {x.TrackMeta}", covers.GetCover(x.Track.ArtworkUri));
               
                trackListItem.Handler = async (item, block) =>
                {
                    await CarPlayPlayerPresenter.PlayAndShowPlayer(
                        tracks.OfType<IMediaTrack>().ToList(),
                        x.Track,
                        this.CreatePlaybackOrigin(),
                        CpInterfaceController);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                tracksCpListItemTemplates.Add(trackListItem);
            });
        
        var section = new CPListSection(tracksCpListItemTemplates.ToArray());
        _favouritesListTemplate.SafeUpdateSections(section.EncloseInArray());
    }
}