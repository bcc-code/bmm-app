using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class BrowseLayoutCreator : IBrowseLayoutCreator
{
    private readonly IBrowseClient _browseClient;
    private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;

    public BrowseLayoutCreator(
        IBrowseClient browseClient,
        IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction)
    {
        _browseClient = browseClient;
        _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        var browseItems = await _browseClient.Get(CachePolicy.UseCacheAndRefreshOutdated);
        var carouselAdjustedItems = await _prepareCoversCarouselItemsAction.ExecuteGuarded(browseItems.ToList());

        var imageRowItemsList = new List<CPListImageRowItem>();
        DiscoverSectionHeader? currentHeader = null;
        var currentItems = new List<ImageRowItem>();

        foreach (var entry in carouselAdjustedItems)
        {
            switch (entry)
            {
                case DiscoverSectionHeader header:
                    AddGroupIfNeeded();
                    currentHeader = header;
                    break;

                case CoverCarouselCollection collection:
                    foreach (var cover in collection.CoverDocuments)
                    {
                        var displayable = (ITrackListDisplayable)cover;
                        var image = await ImageService.Instance
                            .LoadUrl(displayable.Cover)
                            .AsUIImageAsync();

                        currentItems.Add(new ImageRowItem(image, displayable.Title));
                    }
                    break;
            }
        }

        AddGroupIfNeeded();

        var browseListTemplate = new CPListTemplate("Browse", new CPListSection(imageRowItemsList.OfType<ICPListTemplateItem>().ToArray()).EncloseInArray());
        browseListTemplate.TabTitle = "Browse";
        browseListTemplate.TabImage = UIImage.FromBundle("icon_browse".ToNameWithExtension());
        return browseListTemplate;
        
        void AddGroupIfNeeded()
        {
            if (currentHeader == null || !currentItems.Any())
                return;
            
            imageRowItemsList.Add(new CPListImageRowItem(
                currentHeader.Title,
                currentItems.Select(i => i.Image).ToArray(),
                currentItems.Select(i => i.Title).ToArray()));

            currentItems.Clear();
        }
    }
    
    public record ImageRowItem(UIImage Image, string Title);
}