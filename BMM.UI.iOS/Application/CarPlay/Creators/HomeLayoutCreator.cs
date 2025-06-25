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
using BMM.Core.Implementations.FirebaseRemoteConfig;
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
    private IFirebaseRemoteConfig FirebaseRemoteConfig => Mvx.IoCProvider!.Resolve<IFirebaseRemoteConfig>();
    
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
        int currentPodcastId = FirebaseRemoteConfig.CurrentPodcastId;
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
            trackListItems.AddRange(await GetListItems(CpInterfaceController, group.Documents, covers, currentPodcastId));
            sections.Add(new CPListSection(trackListItems.ToArray(), group.Title, null));
        }
        
        _homeTemplate.SafeUpdateSections(sections.ToArray());
    }

    private async Task<IList<ICPListTemplateItem>> GetListItems(CPInterfaceController cpInterfaceController,
        IEnumerable<Document> documents,
        IDictionary<string, UIImage> covers,
        int currentPodcastId)
    {
        var listOfTemplateItems = new List<ICPListTemplateItem>();

        foreach (var document in documents)
        {
            switch (document)
            {
                case ContinueListeningTile continueListeningTile:
                {
                    listOfTemplateItems.Add(await CreateItemForTile(cpInterfaceController, continueListeningTile, covers));
                    listOfTemplateItems.AddIf(
                        () => currentPodcastId == continueListeningTile.ShufflePodcastId,
                        CreateListItem(
                            BMMLanguageBinder[Translations.PodcastViewModel_AllEpisodes],
                            covers.GetCover(continueListeningTile.CoverUrl),
                            () => PodcastLayoutCreator.Create(cpInterfaceController, continueListeningTile.ShufflePodcastId!.Value, continueListeningTile.Title),
                            CpInterfaceController));
                    break;
                }
                case Playlist playlist:
                {
                    listOfTemplateItems.Add(CreateListItem(
                        playlist.Title,
                        covers.GetCover(playlist.Cover),
                        () => PlaylistLayoutCreator.Create(cpInterfaceController, playlist.Id, playlist.Title),
                        cpInterfaceController
                    ));
                    break;
                }
                case Album album:
                {
                    listOfTemplateItems.Add(CreateListItem(
                        album.Title,
                        covers.GetCover(album.Cover),
                        () => AlbumLayoutCreator.Create(cpInterfaceController, album.Id, album.Title),
                        cpInterfaceController
                    ));
                    break;
                }
                case Podcast podcast:
                {
                    listOfTemplateItems.Add(CreateListItem(
                        podcast.Title,
                        covers.GetCover(podcast.Cover),
                        () => PodcastLayoutCreator.Create(cpInterfaceController, podcast.Id, podcast.Title),
                        cpInterfaceController
                    ));
                    break;
                }
                case Contributor contributor:
                {
                    listOfTemplateItems.Add(CreateListItem(
                        contributor.Name,
                        covers.GetCover(contributor.Cover),
                        () => ContributorLayoutCreator.Create(cpInterfaceController, contributor.Id, contributor.Name),
                        cpInterfaceController
                    ));
                    break;
                }
            }
        }

        return listOfTemplateItems;
    }
    
    private CPListItem CreateListItem(
        string title,
        UIImage? image,
        Func<Task<CPListTemplate>> createLayoutFunc,
        CPInterfaceController cpInterfaceController)
    {
        var listItem = new CPListItem(title, null, image);
        listItem.Handler = async (item, block) =>
        {
            var layout = await createLayoutFunc();
            await cpInterfaceController.PushTemplateAsync(layout, true);
            block();
        };
        listItem!.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
        return listItem;
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
        item!.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
        return item;
    }
}