using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.DeepLinking;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
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
public class BrowseLayoutCreator : BaseLayoutCreator, IBrowseLayoutCreator
{
    private const string BrowsePathRegex = @"https?:\/\/[^\/]+\/(.*)";
    private CPListTemplate _browseListTemplate;
    private CPInterfaceController _cpInterfaceController;
    private IBrowseClient BrowseClient => Mvx.IoCProvider!.Resolve<IBrowseClient>();
    private IPrepareCoversCarouselItemsAction PrepareCoversCarouselItemsAction => Mvx.IoCProvider!.Resolve<IPrepareCoversCarouselItemsAction>();
    private IPodcastLayoutCreator PodcastLayoutCreator => Mvx.IoCProvider!.Resolve<IPodcastLayoutCreator>();
    private IPlaylistLayoutCreator PlaylistLayoutCreator => Mvx.IoCProvider!.Resolve<IPlaylistLayoutCreator>();
    private IAlbumLayoutCreator AlbumLayoutCreator => Mvx.IoCProvider!.Resolve<IAlbumLayoutCreator>();
    private IBrowseDetailsLayoutCreator BrowseDetailsLayoutCreator => Mvx.IoCProvider!.Resolve<IBrowseDetailsLayoutCreator>();
    private IBMMLanguageBinder BMMLanguageBinder => Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>();

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        _cpInterfaceController = cpInterfaceController;
        _browseListTemplate = new CPListTemplate(BMMLanguageBinder[Translations.MenuViewModel_Browse], LoadingSection.Create());
        _browseListTemplate.TabTitle = BMMLanguageBinder[Translations.MenuViewModel_Browse];
        _browseListTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconBrowse.ToNameWithExtension());
        SafeLoad().FireAndForget();
        return _browseListTemplate;
    }

    private string GetBrowsePath(string link)
    {
        var match = Regex.Match(link, BrowsePathRegex);
        return match.Success
            ? match.Groups[1].Value
            : default;
    }
    
    public override async Task Load()
    {
        var browseItems = await BrowseClient.Get(CachePolicy.UseCacheAndRefreshOutdated);
        var carouselAdjustedItems = await PrepareCoversCarouselItemsAction.ExecuteGuarded(browseItems.ToList());

        var imageRowItemsList = new List<CPListImageRowItem>();
        DiscoverSectionHeader currentHeader = null;
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
        _browseListTemplate.UpdateSections(new CPListSection(imageRowItemsList.OfType<ICPListTemplateItem>().ToArray()).EncloseInArray());
        
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
                var browseDetailsLayout = await BrowseDetailsLayoutCreator.Create(_cpInterfaceController, browsePath, coverItemInfo.Title);
                await _cpInterfaceController.PushTemplateAsync(browseDetailsLayout, true);
                block();
            };
            
            item.ListImageRowHandler = async (rowItem, index, block) =>
            {
                var imageRowItem = ((CoverItemInfo)rowItem.UserInfo)!.ImageRowItems[(int)index];
                if (imageRowItem.Document is Podcast podcast)
                {
                    var podcastLayout = await PodcastLayoutCreator.Create(_cpInterfaceController, podcast.Id, podcast.Title);
                    await _cpInterfaceController.PushTemplateAsync(podcastLayout, true);
                }
                else if (imageRowItem.Document is Playlist playlist)
                {
                    var playlistLayout = await PlaylistLayoutCreator.Create(_cpInterfaceController, playlist.Id, playlist.Title);
                    await _cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                }
                else if (imageRowItem.Document is Album album)
                {
                    var playlistLayout = await AlbumLayoutCreator.Create(_cpInterfaceController, album.Id, album.Title);
                    await _cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                }
            };
            
            imageRowItemsList.Add(item);
            currentItems.Clear();
        }
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