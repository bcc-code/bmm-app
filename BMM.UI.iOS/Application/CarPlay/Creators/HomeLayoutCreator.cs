using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.CarPlay.Creators.Base;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class HomeLayoutCreator : BaseLayoutCreator, IHomeLayoutCreator
{
    private IDiscoverClient DiscoverClient => Mvx.IoCProvider!.Resolve<IDiscoverClient>();
    private IBMMLanguageBinder BMMLanguageBinder => Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>();
    private IPodcastLayoutCreator PodcastLayoutCreator => Mvx.IoCProvider!.Resolve<IPodcastLayoutCreator>();
    private IContributorLayoutCreator ContributorLayoutCreator => Mvx.IoCProvider!.Resolve<IContributorLayoutCreator>();
    private IPlaylistLayoutCreator PlaylistLayoutCreator => Mvx.IoCProvider!.Resolve<IPlaylistLayoutCreator>();
    private IAlbumLayoutCreator AlbumLayoutCreator => Mvx.IoCProvider!.Resolve<IAlbumLayoutCreator>();
    private IHandleAutoplayAction HandleAutoplayAction => Mvx.IoCProvider!.Resolve<IHandleAutoplayAction>();
    
    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _homeTemplate;

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        _cpInterfaceController = cpInterfaceController;
        _homeTemplate = new CPListTemplate(BMMLanguageBinder[Translations.MenuViewModel_Home], LoadingSection.Create());
        _homeTemplate.TabTitle = BMMLanguageBinder[Translations.MenuViewModel_Home];
        _homeTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconHome.ToNameWithExtension());
        SafeLoad().FireAndForget();
        return _homeTemplate;
    }
    
    public override async Task Load()
    {
        var discoverItems = (await DiscoverClient.GetDocumentsCarPlay(AppTheme.Light, CachePolicy.UseCacheAndRefreshOutdated))
            .ToList();

        var covers = await discoverItems.DownloadCovers();
        var grouped = new List<GroupedDocuments>();
        GroupedDocuments currentGroup = null;

        foreach (var document in discoverItems)
        {
            if (document is DiscoverSectionHeader continueListeningTile)
            {
                currentGroup = new GroupedDocuments(continueListeningTile.Title);
                grouped.Add(currentGroup);
            }
            else
            {
                if (currentGroup == null)
                {
                    currentGroup = new GroupedDocuments(null);
                    grouped.Add(currentGroup);
                }

                currentGroup.Documents.Add(document);
            }
        }

        IList<CPListSection> sections = new List<CPListSection>();

        foreach (var group in grouped)
        {
            var trackListItems = new List<ICPListTemplateItem>();
            trackListItems.AddRange(await GetTrackListItems(CpInterfaceController, group.Documents, covers));
            sections.Add(new CPListSection(trackListItems.ToArray(), group.Title, null));
        }
        
        _homeTemplate.SafeUpdateSections(sections.ToArray());
    }

    private async Task<IList<ICPListTemplateItem>> GetTrackListItems(
        CPInterfaceController cpInterfaceController,
        IEnumerable<Document> documents,
        IDictionary<string, UIImage> covers)
    {
        return await Task.WhenAll(documents
            .Select(async d =>
            {
                CPListItem trackListItem = null;

                switch (d)
                {
                    case ContinueListeningTile continueListeningTile:
                    {
                        trackListItem = await CreateItemForTile(cpInterfaceController, continueListeningTile, covers);
                        break;
                    }
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
                    case Contributor contributor:
                    {
                        trackListItem = new CPListItem(contributor.Name, null, covers.GetCover(contributor.Cover));
                        trackListItem.Handler = async (item, block) =>
                        {
                            var contributorLayout = await ContributorLayoutCreator.Create(cpInterfaceController, contributor.Id, contributor.Name);
                            await cpInterfaceController.PushTemplateAsync(contributorLayout, true);
                            block();
                        };

                        break;
                    }
                }

                trackListItem!.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                return trackListItem;
            }));
    }
        
    private async Task<CPListItem> CreateItemForTile(CPInterfaceController cpInterfaceController,
        ContinueListeningTile continueListeningTile,
        IDictionary<string, UIImage> covers)
    {
        var dateTimeToPodcastPublishDateLabelValueConverter = new DateTimeToPodcastPublishDateLabelValueConverter();
        var dateTimeToPodcastPublishDayOfWeekLabelValueConverter = new DateTimeToPodcastPublishDayOfWeekLabelValueConverter();
        var subtitle = new StringBuilder(continueListeningTile.Title);
        
        string dateOfWeek = (string)dateTimeToPodcastPublishDayOfWeekLabelValueConverter
            .Convert(continueListeningTile.Date.Value,
                typeof(string),
                null,
                CultureInfo.CurrentUICulture);
        string dateLabel = (string)dateTimeToPodcastPublishDateLabelValueConverter
            .Convert(continueListeningTile.Date.Value,
                typeof(string),
                null,
                CultureInfo.CurrentUICulture);

        subtitle.Append($" - {dateOfWeek}, {dateLabel}");

        var item = new CPListItem(continueListeningTile.Label, subtitle.ToString(), covers.GetCover(continueListeningTile.CoverUrl));
        item.Handler = async (_, block) =>
        {
            await CarPlayPlayerPresenter.PlayAndShowPlayer(
                continueListeningTile.Track.ToTracksList(),
                continueListeningTile.Track,
                this.CreatePlaybackOrigin(),
                cpInterfaceController);
            await HandleAutoplayAction.ExecuteGuarded(continueListeningTile);
            block();
        };
        return item;
    }
}