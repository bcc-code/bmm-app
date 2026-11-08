using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.Translation;
using BMM.UI.iOS.CarPlay.Creators.Base;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class FollowedPodcastsContentLayoutCreator : BaseLayoutCreator, IFollowedPodcastsContentLayoutCreator
{
    private IPodcastClient PodcastClient => Mvx.IoCProvider!.Resolve<IPodcastClient>();
    private IPodcastOfflineManager PodcastOfflineManager => Mvx.IoCProvider!.Resolve<IPodcastOfflineManager>();
    private IBMMLanguageBinder BMMLanguageBinder => Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>();
    private IPodcastLayoutCreator PodcastLayoutCreator => Mvx.IoCProvider!.Resolve<IPodcastLayoutCreator>();

    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _followedPodcastsListTemplate;

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        _cpInterfaceController = cpInterfaceController; 
        _followedPodcastsListTemplate = new CPListTemplate(BMMLanguageBinder[Translations.DownloadedContentViewModel_FollowedPodcasts], LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _followedPodcastsListTemplate;
    }

    public override async Task Load()
    {
        var podcasts = await PodcastClient.GetAll(CachePolicy.UseCacheAndWaitForUpdates);
        var followedPodcasts = podcasts?
            .Where(PodcastOfflineManager.IsFollowing)
            .Select(p => new PodcastPO(p))
            .ToList();

        var covers = await followedPodcasts.DownloadCovers();
        
        var tracklistItems = await Task.WhenAll(followedPodcasts
            .Select(async p =>
            {
                var trackListItem = new CPListItem(p.Title, null, covers.GetCover(p.Cover));
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                
                trackListItem.Handler = async (item, block) =>
                {
                    var playlistLayout = await PodcastLayoutCreator.Create(CpInterfaceController, p.Id, p.Title);
                    await CpInterfaceController.PushTemplateAsync(playlistLayout, true);
                    block();
                };
                
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracklistItems);
        _followedPodcastsListTemplate.SafeUpdateSections(section.EncloseInArray());
    }
}