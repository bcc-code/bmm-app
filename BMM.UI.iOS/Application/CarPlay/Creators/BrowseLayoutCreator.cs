using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.DeepLinking;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class BrowseLayoutCreator : IBrowseLayoutCreator
{
    private const string BrowsePathRegex = @"https?:\/\/[^\/]+\/(.*)";
    private readonly IBrowseClient _browseClient;
    private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;
    private readonly IPodcastLayoutCreator _podcastLayoutCreator;
    private readonly IPlaylistLayoutCreator _playlistLayoutCreator;
    private readonly IAlbumLayoutCreator _albumLayoutCreator;
    private readonly IBrowseDetailsLayoutCreator _browseDetailsLayoutCreator;

    public BrowseLayoutCreator(
        IBrowseClient browseClient,
        IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
        IPodcastLayoutCreator podcastLayoutCreator,
        IPlaylistLayoutCreator playlistLayoutCreator,
        IAlbumLayoutCreator albumLayoutCreator,
        IBrowseDetailsLayoutCreator browseDetailsLayoutCreator)
    {
        _browseClient = browseClient;
        _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
        _podcastLayoutCreator = podcastLayoutCreator;
        _playlistLayoutCreator = playlistLayoutCreator;
        _albumLayoutCreator = albumLayoutCreator;
        _browseDetailsLayoutCreator = browseDetailsLayoutCreator;
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
                        var image = await displayable.Cover.ToUIImage();
                        currentItems.Add(new ImageRowItem(image, displayable.Title, cover));
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

            var item = new CPListImageRowItem(
                currentHeader.Title,
                currentItems.Select(i => i.Image).ToArray(),
                currentItems.Select(i => i.Title).ToArray());

            item.UserInfo = new CoverItemInfo(currentItems.ToList(), currentHeader.Title, currentHeader.Link);
            item.Handler = async (listItem, block) =>
            {
                var coverItemInfo = (CoverItemInfo)listItem.UserInfo;
                string browsePath = GetBrowsePath(coverItemInfo.Link);
                var browseDetailsLayout = await _browseDetailsLayoutCreator.Create(cpInterfaceController, browsePath, coverItemInfo.Title);
                await cpInterfaceController.PushTemplateAsync(browseDetailsLayout, true);
                block();
            };
            
            item.ListImageRowHandler = async (rowItem, index, block) =>
            {
                var imageRowItem = ((CoverItemInfo)rowItem.UserInfo)!.ImageRowItems[(int)index];
                if (imageRowItem.Document is Podcast podcast)
                {
                    var podcastLayout = await _podcastLayoutCreator.Create(cpInterfaceController, podcast.Id, podcast.Title);
                    await cpInterfaceController.PushTemplateAsync(podcastLayout, true);
                }
                else if (imageRowItem.Document is Playlist playlist)
                {
                    var playlistLayout = await _playlistLayoutCreator.Create(cpInterfaceController, playlist.Id, playlist.Title);
                    await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                }
                else if (imageRowItem.Document is Album album)
                {
                    var playlistLayout = await _albumLayoutCreator.Create(cpInterfaceController, album.Id, album.Title);
                    await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                }
            };
            
            imageRowItemsList.Add(item);
            currentItems.Clear();
        }
    }

    private string GetBrowsePath(string link)
    {
        var match = Regex.Match(link, BrowsePathRegex);
        return match.Success
            ? match.Groups[1].Value
            : default;
    }

    public record ImageRowItem(
        UIImage Image,
        string Title,
        Document Document);

    public class CoverItemInfo : NSObject
    {
        public CoverItemInfo(IList<ImageRowItem> imageRowItems, string title, string link)
        {
            ImageRowItems = imageRowItems;
            Title = title;
            Link = link;
        }
        
        public IList<ImageRowItem> ImageRowItems { get; }
        public string Title { get; }
        public string Link { get; }
    }
}