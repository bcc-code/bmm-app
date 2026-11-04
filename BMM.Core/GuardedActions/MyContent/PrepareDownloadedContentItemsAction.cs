using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ViewModels.MyContent;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.MyContent;

public class PrepareDownloadedContentItemsAction
    : GuardedActionWithParameterAndResult<IEnumerable<TrackCollectionPO>, IList<DocumentPO>>,
      IPrepareDownloadedContentItemsAction
{
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly IPlaylistClient _playlistClient;
    private readonly IOfflinePlaylistStorage _offlinePlaylistStorage;
    private readonly IPlaylistPOFactory _playlistPOFactory;
    private readonly IPodcastClient _podcastClient;

    public PrepareDownloadedContentItemsAction(
        IBMMLanguageBinder bmmLanguageBinder,
        IMvxNavigationService mvxNavigationService,
        IPlaylistClient playlistClient,
        IOfflinePlaylistStorage offlinePlaylistStorage,
        IPlaylistPOFactory playlistPOFactory,
        IPodcastClient podcastClient)
    {
        _bmmLanguageBinder = bmmLanguageBinder;
        _mvxNavigationService = mvxNavigationService;
        _playlistClient = playlistClient;
        _offlinePlaylistStorage = offlinePlaylistStorage;
        _playlistPOFactory = playlistPOFactory;
        _podcastClient = podcastClient;
    }
    
    protected override async Task<IList<DocumentPO>> Execute(IEnumerable<TrackCollectionPO> documentsCollections)
    {
        // Whether the user asked for a collection offline is the only thing that decides if it belongs
        // here, and that answer cannot depend on being online. This used to additionally require, when
        // offline only, that at least one track pass IsDownloaded - which reports false for any track
        // whose Url is null, something that happens often enough to have its own analytics event. A
        // fully downloaded playlist would therefore be listed while connected and disappear the moment
        // the device went offline, which is precisely when people go looking for it.
        var offlineTrackCollections = documentsCollections?.Where(tc => tc.IsAvailableOffline).ToList();
        var offlinePlaylists = await BuildDownloadedCuratedPlaylists();
        
        var items = new List<DocumentPO>();
        
        items.AddRange(await BuildPinnedItems());
        items.AddRange(offlinePlaylists);
        if (offlineTrackCollections != null)
            items.AddRange(offlineTrackCollections);

        return items.ToList();
    }
    
    private async Task<IEnumerable<DocumentPO>> BuildDownloadedCuratedPlaylists()
    {
        var documents = new List<DocumentPO>();
        IList<Playlist> playlists = await _playlistClient.GetAll(CachePolicy.UseCache);
        var downloadedPlaylistIds = await _offlinePlaylistStorage.GetPlaylistIds();
        foreach (var playlist in playlists.OrderBy(x => x.Title))
        {
            if (downloadedPlaylistIds.Contains(playlist.Id))
                documents.Add(_playlistPOFactory.Create(playlist));
        }

        return documents;
    }

    private async Task<IEnumerable<PinnedItemPO>> BuildPinnedItems()
    {
        var followedPodcastPinnedItem = new PinnedItem
        {
            Title = _bmmLanguageBinder[Translations.DownloadedContentViewModel_FollowedPodcasts],
            Action = new MvxAsyncCommand<PinnedItem>(execute => _mvxNavigationService.Navigate<DownloadedFollowedPodcastsViewModel>()),
            Icon = "icon_podcast",
            ActionType = PinnedItemActionType.DownloadedFollowedPodcasts
        };

        var pinnedItemsList = new List<PinnedItemPO>();

        var podcasts = await _podcastClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);
        var followedPodcasts = podcasts?.Where(Mvx.IoCProvider.Resolve<IPodcastOfflineManager>().IsFollowing);
        if (followedPodcasts != null && followedPodcasts.Any())
            pinnedItemsList.Add(new PinnedItemPO(followedPodcastPinnedItem));

        return pinnedItemsList;
    }
}