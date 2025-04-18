using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Security;
using BMM.Core.Models.POs.Albums;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class BrowseDetailsLayoutCreator : IBrowseDetailsLayoutCreator
{
    private const int Skip = 0;
    private const int Take = 40;
    private readonly IBrowseClient _browseClient;
    private readonly IPlaylistClient _playlistClient;
    private readonly IPlaylistLayoutCreator _playlistLayoutCreator;
    private readonly IAlbumLayoutCreator _albumLayoutCreator;
    private readonly IPodcastLayoutCreator _podcastLayoutCreator;
    private readonly IUserStorage _userStorage;

    public BrowseDetailsLayoutCreator(
        IBrowseClient browseClient,
        IPlaylistClient playlistClient,
        IPlaylistLayoutCreator playlistLayoutCreator,
        IAlbumLayoutCreator albumLayoutCreator,
        IPodcastLayoutCreator podcastLayoutCreator,
        IUserStorage userStorage)
    {
        _browseClient = browseClient;
        _playlistClient = playlistClient;
        _playlistLayoutCreator = playlistLayoutCreator;
        _albumLayoutCreator = albumLayoutCreator;
        _podcastLayoutCreator = podcastLayoutCreator;
        _userStorage = userStorage;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, string browsePath, string title)
    {
        var browseDetailsListTemplates = new CPListTemplate(title, LoadingSection.Create());
        Load(cpInterfaceController, browseDetailsListTemplates, browsePath).FireAndForget();
        return browseDetailsListTemplates;
    }

    private async Task Load(
        CPInterfaceController cpInterfaceController,
        CPListTemplate browseDetailsListTemplate,
        string browsePath)
    {
        GenericDocumentsHolder documentsHolder;
        
        if (browsePath.Contains("featured"))
        {
            documentsHolder = await _playlistClient
                .GetDocuments(_userStorage.GetUser().Age, CachePolicy.UseCacheAndRefreshOutdated);
        }
        else
        {
            documentsHolder = await _browseClient.GetDocuments(browsePath, Skip, Take);
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

        CPListSection[] sections;
        
        if (grouped.Any())
        {
            sections = await Task.WhenAll(grouped
                .Select(async grouped =>
                {
                    var trackListItems = new List<ICPListTemplateItem>();
                    trackListItems.AddRange(await GetTrackListItems(cpInterfaceController, grouped.Documents));
                    return new CPListSection(trackListItems.ToArray(), grouped.Title, null);
                })
                .ToArray());
        }
        else
        {
            sections = await Task.WhenAll(documentsHolder.Items
                .Select(async document =>
                {
                    var trackListItems = new List<ICPListTemplateItem>();
                    trackListItems.AddRange(await GetTrackListItems(cpInterfaceController, documentsHolder.Items));
                    return new CPListSection(trackListItems.ToArray());
                })
                .ToArray());
        }
        
        browseDetailsListTemplate.UpdateSections(sections);
    }

    private async Task<IList<ICPListTemplateItem>> GetTrackListItems(CPInterfaceController cpInterfaceController, IEnumerable<Document> documents)
    {
        return await Task.WhenAll(documents
            .Select(async d =>
            {
                CPListItem trackListItem = null;

                switch (d)
                {
                    case Playlist playlist:
                    {
                        var coverImage = await playlist.Cover.ToUIImage();
                        trackListItem = new CPListItem(playlist.Title, null, coverImage);
                        trackListItem.Handler = async (item, block) =>
                        {
                            var playlistLayout = await _playlistLayoutCreator.Create(cpInterfaceController, playlist.Id, playlist.Title);
                            await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                            block();
                        };

                        break;
                    }
                    case Album album:
                    {
                        var coverImage = await album.Cover.ToUIImage();
                        trackListItem = new CPListItem(album.Title, null, coverImage);
                        trackListItem.Handler = async (item, block) =>
                        {
                            var playlistLayout = await _albumLayoutCreator.Create(cpInterfaceController, album.Id, album.Title);
                            await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                            block();
                        };

                        break;
                    }
                    case Podcast podcast:
                    {
                        var coverImage = await podcast.Cover.ToUIImage();
                        trackListItem = new CPListItem(podcast.Title, null, coverImage);
                        trackListItem.Handler = async (item, block) =>
                        {
                            var podcastLayout = await _podcastLayoutCreator.Create(cpInterfaceController, podcast.Id, podcast.Title);
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