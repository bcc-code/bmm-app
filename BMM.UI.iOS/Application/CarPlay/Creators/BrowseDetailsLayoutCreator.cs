using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Security;
using BMM.Core.Models.POs.Albums;
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
public class BrowseDetailsLayoutCreator : BaseLayoutCreator, IBrowseDetailsLayoutCreator
{
    private const int Skip = 0;
    private const int Take = 40;
    private const string FeaturedBrowsePath = "featured";
    
    private IBrowseClient BrowseClient => Mvx.IoCProvider!.Resolve<IBrowseClient>();
    private IPlaylistClient PlaylistClient => Mvx.IoCProvider!.Resolve<IPlaylistClient>();
    private IPlaylistLayoutCreator PlaylistLayoutCreator => Mvx.IoCProvider!.Resolve<IPlaylistLayoutCreator>();
    private IAlbumLayoutCreator AlbumLayoutCreator => Mvx.IoCProvider!.Resolve<IAlbumLayoutCreator>();
    private IPodcastLayoutCreator PodcastLayoutCreator => Mvx.IoCProvider!.Resolve<IPodcastLayoutCreator>();
    private IUserStorage UserStorage => Mvx.IoCProvider!.Resolve<IUserStorage>();
    
    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _browseDetailsListTemplates;
    private string _browsePath;

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, string browsePath, string title)
    {
        _cpInterfaceController = cpInterfaceController;
        _browsePath = browsePath;
        _browseDetailsListTemplates = new CPListTemplate(title, LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _browseDetailsListTemplates;
    }

    public override async Task Load()
    {
        GenericDocumentsHolder documentsHolder;

        if (_browsePath.Contains(FeaturedBrowsePath))
        {
            documentsHolder = await PlaylistClient
                .GetDocuments(UserStorage.GetUser().Age, CachePolicy.UseCacheAndWaitForUpdates);
        }
        else
        {
            documentsHolder = await BrowseClient.GetDocuments(_browsePath, Skip, Take);
        }

        var grouped = new List<GroupedDocuments>();
        GroupedDocuments currentGroup = null;

        foreach (var entry in documentsHolder.Items)
        {
            if (entry is ChapterHeader chapterHeader)
            {
                currentGroup = new GroupedDocuments(chapterHeader.Title);
                grouped.Add(currentGroup);
            }
            else if (entry is DiscoverSectionHeader discoverSectionHeader)
            {
                currentGroup = new GroupedDocuments(discoverSectionHeader.Title);
                grouped.Add(currentGroup);
            }
            else if (currentGroup != null)
            {
                currentGroup.Documents.Add(entry);
            }
        }

        var covers = await documentsHolder
            .Items
            .DownloadCovers();
        
        CPListSection[] sections;

        if (grouped.Any())
        {
            sections = await Task.WhenAll(grouped
                .Select(async grouped =>
                {
                    var trackListItems = new List<ICPListTemplateItem>();
                    trackListItems.AddRange(await GetTrackListItems(CpInterfaceController, grouped.Documents, covers));
                    return new CPListSection(trackListItems.ToArray(), grouped.Title, null);
                })
                .ToArray());
        }
        else
        {
            var trackListItems = new List<ICPListTemplateItem>();
            trackListItems.AddRange(await GetTrackListItems(CpInterfaceController, documentsHolder.Items, covers));
            sections = [new CPListSection(trackListItems.ToArray())];
        }

        _browseDetailsListTemplates.SafeUpdateSections(sections);
    }

    private async Task<IList<ICPListTemplateItem>> GetTrackListItems(CPInterfaceController cpInterfaceController,
        IEnumerable<Document> documents,
        IDictionary<string, UIImage> covers)
    {
        return await Task.WhenAll(documents
            .Select(async d =>
            {
                CPListItem trackListItem = null;

                switch (d)
                {
                    case Playlist playlist:
                    {
                        trackListItem = new CPListItem(playlist.Title, null, covers.GetCover(playlist.Cover));
                        trackListItem.Handler = async (item, block) =>
                        {
                            var playlistLayout = await PlaylistLayoutCreator.Create(cpInterfaceController, playlist.Id, playlist.Title);
                            await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                            block();
                        };

                        break;
                    }
                    case Album album:
                    {
                        trackListItem = new CPListItem(album.Title, null, covers.GetCover(album.Cover));
                        trackListItem.Handler = async (item, block) =>
                        {
                            var playlistLayout = await AlbumLayoutCreator.Create(cpInterfaceController, album.Id, album.Title);
                            await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                            block();
                        };

                        break;
                    }
                    case Podcast podcast:
                    {
                        trackListItem = new CPListItem(podcast.Title, null, covers.GetCover(podcast.Cover));
                        trackListItem.Handler = async (item, block) =>
                        {
                            var podcastLayout = await PodcastLayoutCreator.Create(cpInterfaceController, podcast.Id, podcast.Title);
                            await cpInterfaceController.PushTemplateAsync(podcastLayout, true);
                            block();
                        };

                        break;
                    }
                }

                trackListItem!.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                return trackListItem;
            }));
    }
}

public class GroupedDocuments
{
    public GroupedDocuments(string title)
    {
        Title = title;
    }
        
    public string Title { get; }
    public IList<Document> Documents = new List<Document>();
}