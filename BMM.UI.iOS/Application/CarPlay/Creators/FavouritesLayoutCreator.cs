using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.UI.iOS.CarPlay.Creators.Base;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class FavouritesLayoutCreator : BaseLayoutCreator, IFavouritesLayoutCreator
{
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly ITrackCollectionClient _trackCollectionClient;
    private readonly ITrackCollectionPOFactory _trackCollectionPOFactory;
    private readonly IPrepareMyContentItemsAction _prepareMyContentItemsAction;
    private readonly IDownloadedContentLayoutCreator _downloadedContentLayoutCreator;
    private readonly IFollowedPodcastsContentLayoutCreator _followedPodcastsContentLayoutCreator;
    private readonly ITrackCollectionContentLayoutCreator _trackCollectionContentLayoutCreator;
    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _favouritesListTemplate;

    public FavouritesLayoutCreator(
        IBMMLanguageBinder bmmLanguageBinder,
        ITrackCollectionClient trackCollectionClient,
        ITrackCollectionPOFactory trackCollectionPOFactory,
        IPrepareMyContentItemsAction prepareMyContentItemsAction,
        IDownloadedContentLayoutCreator downloadedContentLayoutCreator,
        IFollowedPodcastsContentLayoutCreator followedPodcastsContentLayoutCreator,
        ITrackCollectionContentLayoutCreator trackCollectionContentLayoutCreator)
    {
        _bmmLanguageBinder = bmmLanguageBinder;
        _trackCollectionClient = trackCollectionClient;
        _trackCollectionPOFactory = trackCollectionPOFactory;
        _prepareMyContentItemsAction = prepareMyContentItemsAction;
        _downloadedContentLayoutCreator = downloadedContentLayoutCreator;
        _followedPodcastsContentLayoutCreator = followedPodcastsContentLayoutCreator;
        _trackCollectionContentLayoutCreator = trackCollectionContentLayoutCreator;
    }

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        _cpInterfaceController = cpInterfaceController;
        _favouritesListTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.MenuViewModel_Favorites], LoadingSection.Create());
        _favouritesListTemplate.TabTitle = _bmmLanguageBinder[Translations.MenuViewModel_Favorites];
        _favouritesListTemplate.TabImage = UIImage.FromBundle("icon_favorites".ToNameWithExtension());
        SafeLoad().FireAndForget();
        return _favouritesListTemplate;
    }

    public override async Task Load()
    {
        var allCollections = await _trackCollectionClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);

        var items =  allCollections
            .OrderByDescending(c => c.Id)
            .Select(tc => _trackCollectionPOFactory.Create(tc))
            .ToList();

        var myContentItems = await _prepareMyContentItemsAction.ExecuteGuarded(items);
        
        var converter = new TrackCollectionToListViewItemSubtitleLabelConverter();

        var tracklistItems = myContentItems
            .Where(m => m is not ChapterHeaderPO)
            .Select(x =>
            {
                CPListItem trackListItem = null;

                switch (x)
                {
                    case PinnedItemPO pinnedItemPO:
                    {
                        var image = pinnedItemPO.PinnedItem.ActionType switch
                        {
                            PinnedItemActionType.DownloadedContent => UIImage.FromBundle(ImageResourceNames.IconDownloadedCarplay
                                .ToIosImageName()),
                            PinnedItemActionType.FollowedPodcasts => UIImage.FromBundle(ImageResourceNames.IconFollowedPodcastsCarplay
                                .ToIosImageName()),
                            _ => null
                        };

                        trackListItem = new CPListItem(pinnedItemPO.PinnedItem.Title, null, image);
                        trackListItem.Handler = async (item, block) =>
                        {
                            if (pinnedItemPO.PinnedItem.ActionType == PinnedItemActionType.DownloadedContent)
                            {
                                var downloadedContentTemplate = await _downloadedContentLayoutCreator.Create(_cpInterfaceController);
                                await _cpInterfaceController.PushTemplateAsync(downloadedContentTemplate, true);
                            }
                            else if (pinnedItemPO.PinnedItem.ActionType == PinnedItemActionType.FollowedPodcasts)
                            {
                                var followedPodcastsContentLayout = await _followedPodcastsContentLayoutCreator.Create(_cpInterfaceController);
                                await _cpInterfaceController.PushTemplateAsync(followedPodcastsContentLayout, true);
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
                    
                        trackListItem = new CPListItem(trackCollectionPO.TrackCollection.Name, detailsText, UIImage.FromBundle(ImageResourceNames.IconPlaylistCarplay.ToIosImageName()));
                        trackListItem.Handler = async (item, block) =>
                        {
                            var trackCollectionContentLayout = await _trackCollectionContentLayoutCreator.Create(
                                _cpInterfaceController,
                                trackCollectionPO.TrackCollection.Name,
                                trackCollectionPO.Id);
                            await _cpInterfaceController.PushTemplateAsync(trackCollectionContentLayout, true);
                            block();
                        };
                        
                        break;
                    }
                }
                
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                return trackListItem;
            })
            .ToList();
        
        var section = new CPListSection(tracklistItems.OfType<ICPListTemplateItem>().ToArray());
        _favouritesListTemplate.UpdateSections(section.EncloseInArray());
    }
}