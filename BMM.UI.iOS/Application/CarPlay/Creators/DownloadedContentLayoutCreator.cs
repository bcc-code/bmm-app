using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class DownloadedContentLayoutCreator : IDownloadedContentLayoutCreator
{
    private readonly ITrackCollectionClient _trackCollectionClient;
    private readonly ITrackCollectionPOFactory _trackCollectionPOFactory;
    private readonly IPrepareDownloadedContentItemsAction _prepareDownloadedContentItemsAction;
    private readonly IStorageManager _storageManager;
    private readonly IFollowedPodcastsContentLayoutCreator _followedPodcastsContentLayoutCreator;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly ITrackCollectionContentLayoutCreator _trackCollectionContentLayoutCreator;
    private readonly IPlaylistLayoutCreator _playlistLayoutCreator;

    public DownloadedContentLayoutCreator(
        ITrackCollectionClient trackCollectionClient,
        ITrackCollectionPOFactory trackCollectionPOFactory,
        IPrepareDownloadedContentItemsAction prepareDownloadedContentItemsAction,
        IStorageManager storageManager,
        IFollowedPodcastsContentLayoutCreator followedPodcastsContentLayoutCreator,
        IBMMLanguageBinder bmmLanguageBinder,
        ITrackCollectionContentLayoutCreator trackCollectionContentLayoutCreator,
        IPlaylistLayoutCreator playlistLayoutCreator)
    {
        _trackCollectionClient = trackCollectionClient;
        _trackCollectionPOFactory = trackCollectionPOFactory;
        _prepareDownloadedContentItemsAction = prepareDownloadedContentItemsAction;
        _storageManager = storageManager;
        _followedPodcastsContentLayoutCreator = followedPodcastsContentLayoutCreator;
        _bmmLanguageBinder = bmmLanguageBinder;
        _trackCollectionContentLayoutCreator = trackCollectionContentLayoutCreator;
        _playlistLayoutCreator = playlistLayoutCreator;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        var downloadedListTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.DownloadedContentViewModel_Title], LoadingSection.Create());
        Load(cpInterfaceController, downloadedListTemplate).FireAndForget();
        return downloadedListTemplate;
    }

    private async Task Load(CPInterfaceController cpInterfaceController, CPListTemplate downloadedListTemplate)
    {
        var allCollections = await _trackCollectionClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);

        var items =  allCollections
            .OrderByDescending(c => c.Id)
            .Select(tc => _trackCollectionPOFactory.Create(tc))
            .ToList();
        
        var downloadedContentItems = await _prepareDownloadedContentItemsAction.ExecuteGuarded(items);
        var converter = new TrackCollectionToListViewItemSubtitleLabelConverter();

        var tracklistItems = await Task.WhenAll(downloadedContentItems
            .Select(async x =>
            {
                CPListItem trackListItem = null;

                switch (x)
                {
                    case PinnedItemPO pinnedItemPO:
                    {
                        var image = UIImage.FromBundle(ImageResourceNames.IconFollowedPodcastsCarplay.ToIosImageName());
                        trackListItem = new CPListItem(pinnedItemPO.PinnedItem.Title, null, image);
                        trackListItem.Handler = async (item, block) =>
                        {
                            if (pinnedItemPO.PinnedItem.ActionType == PinnedItemActionType.DownloadedFollowedPodcasts)
                            {
                                var followedPodcastsContentLayout = await _followedPodcastsContentLayoutCreator.Create(cpInterfaceController);
                                await cpInterfaceController.PushTemplateAsync(followedPodcastsContentLayout, true);
                            }

                            block();
                        };

                        break;
                    }
                    case TrackCollectionPO trackCollectionPO:
                    {
                        string detailsText = (string)converter.Convert(
                            trackCollectionPO.TrackCollection,
                            typeof(string),
                            null,
                            CultureInfo.CurrentUICulture);

                        trackListItem = new CPListItem(trackCollectionPO.TrackCollection.Name, detailsText);
                        trackListItem.Handler = async (item, block) =>
                        {
                            var trackCollectionContentLayout = await _trackCollectionContentLayoutCreator.Create(
                                cpInterfaceController,
                                trackCollectionPO.TrackCollection.Name,
                                trackCollectionPO.Id);
                            await cpInterfaceController.PushTemplateAsync(trackCollectionContentLayout, true);
                            block();
                        };

                        break;
                    }
                    case PlaylistPO playlistPO:
                    {
                        var coverImage = await ImageService
                            .Instance
                            .LoadUrl(playlistPO.Cover)
                            .AsUIImageAsync();

                        trackListItem = new CPListItem(playlistPO.Title, null, coverImage);
                        trackListItem.Handler = async (item, block) =>
                        {
                            var playlistLayout = await _playlistLayoutCreator.Create(cpInterfaceController, playlistPO.Id, playlistPO.Title);
                            await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                            block();
                        };

                        break;
                    }
                    case AlbumPO albumPO:
                    {
                        var coverImage = await ImageService
                            .Instance
                            .LoadUrl(albumPO.Cover)
                            .AsUIImageAsync();

                        trackListItem = new CPListItem(albumPO.Title, null, coverImage);
                        trackListItem.Handler = async (item, block) =>
                        {
                            block();
                        };

                        break;
                    }
                }

                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                return (ICPListTemplateItem)trackListItem;
            }));
        
        var section = new CPListSection(tracklistItems);
        downloadedListTemplate.UpdateSections(section.EncloseInArray());
    }
}