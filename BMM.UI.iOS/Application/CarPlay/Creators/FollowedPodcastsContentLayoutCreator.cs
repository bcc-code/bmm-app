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

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class FollowedPodcastsContentLayoutCreator : BaseLayoutCreator, IFollowedPodcastsContentLayoutCreator
{
    private readonly IPodcastClient _podcastClient;
    private readonly IPodcastOfflineManager _podcastOfflineManager;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IStorageManager _storageManager;
    private readonly IPodcastLayoutCreator _podcastLayoutCreator;
    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _followedPodcastsListTemplate;

    public FollowedPodcastsContentLayoutCreator(
        IPodcastClient podcastClient,
        IPodcastOfflineManager podcastOfflineManager,
        IBMMLanguageBinder bmmLanguageBinder,
        IStorageManager storageManager,
        IPodcastLayoutCreator podcastLayoutCreator)
    {
        _podcastClient = podcastClient;
        _podcastOfflineManager = podcastOfflineManager;
        _bmmLanguageBinder = bmmLanguageBinder;
        _storageManager = storageManager;
        _podcastLayoutCreator = podcastLayoutCreator;
    }

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        _cpInterfaceController = cpInterfaceController; 
        _followedPodcastsListTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.DownloadedContentViewModel_FollowedPodcasts], LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _followedPodcastsListTemplate;
    }

    public override async Task Load()
    {
        var podcasts = await _podcastClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);
        var followedPodcasts = podcasts?
            .Where(_podcastOfflineManager.IsFollowing)
            .Select(p => new PodcastPO(p))
            .ToList();
        
        var tracklistItems = await Task.WhenAll(followedPodcasts
            .Select(async p =>
            {
                var coverImage = await p.Cover.ToUIImage();
                var trackListItem = new CPListItem(p.Title, null, coverImage);
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                
                trackListItem.Handler = async (item, block) =>
                {
                    var playlistLayout = await _podcastLayoutCreator.Create(CpInterfaceController, p.Id, p.Title);
                    await CpInterfaceController.PushTemplateAsync(playlistLayout, true);
                    block();
                };
                
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracklistItems);
        _followedPodcastsListTemplate.UpdateSections(section.EncloseInArray());
    }
}