using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class FavouritesLayoutCreator : IFavouritesLayoutCreator
{
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly ITrackCollectionClient _trackCollectionClient;
    private readonly ITrackCollectionPOFactory _trackCollectionPOFactory;
    private readonly IPrepareMyContentItemsAction _prepareMyContentItemsAction;

    public FavouritesLayoutCreator(
        IBMMLanguageBinder bmmLanguageBinder,
        ITrackCollectionClient trackCollectionClient,
        ITrackCollectionPOFactory trackCollectionPOFactory,
        IPrepareMyContentItemsAction prepareMyContentItemsAction)
    {
        _bmmLanguageBinder = bmmLanguageBinder;
        _trackCollectionClient = trackCollectionClient;
        _trackCollectionPOFactory = trackCollectionPOFactory;
        _prepareMyContentItemsAction = prepareMyContentItemsAction;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        await Task.CompletedTask;
        
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

                if (x is PinnedItemPO pinnedItemPO)
                {
                    trackListItem = new CPListItem(pinnedItemPO.PinnedItem.Title, null);
                }
                else if (x is TrackCollectionPO trackCollectionPO)
                {
                    trackListItem = new CPListItem(trackCollectionPO.TrackCollection.Name, (string)converter.Convert(
                        trackCollectionPO.TrackCollection,
                        typeof(string),
                        null,
                        CultureInfo.CurrentUICulture));
                }
                
                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;

                trackListItem.Handler = async (item, block) =>
                {
                    // await _mediaPlayer.Play(tracks.OfType<IMediaTrack>().ToList(), x.Track);
                    // var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                    // await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };
                
                return trackListItem;
            })
            .ToList();
        
        var section = new CPListSection(tracklistItems.OfType<ICPListTemplateItem>().ToArray());
        var favouritesListTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.MenuViewModel_Favorites], section.EncloseInArray());
        favouritesListTemplate.TabTitle = _bmmLanguageBinder[Translations.MenuViewModel_Favorites];
        favouritesListTemplate.TabImage = UIImage.FromBundle("icon_favorites".ToNameWithExtension());
        return favouritesListTemplate;
    }
}