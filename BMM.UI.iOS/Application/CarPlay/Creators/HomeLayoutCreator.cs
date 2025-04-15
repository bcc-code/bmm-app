using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using CarPlay;
using FFImageLoading;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class HomeLayoutCreator : IHomeLayoutCreator
{
    private readonly IDiscoverClient _discoverClient;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly ITrackPOFactory _trackFactory;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IUserStorage _user;
    private readonly IPrepareCoversCarouselItemsAction _prepareCarouselItemsAction;
    private readonly IPodcastLayoutCreator _podcastLayoutCreator;
    private readonly IContributorLayoutCreator _contributorLayoutCreator;
    private readonly IPlaylistLayoutCreator _playlistLayoutCreator;
    private readonly IAlbumLayoutCreator _albumLayoutCreator;

    public HomeLayoutCreator(
        IDiscoverClient discoverClient,
        IMediaPlayer mediaPlayer,
        ITrackPOFactory trackFactory,
        IBMMLanguageBinder bmmLanguageBinder,
        IUserStorage user,
        IPrepareCoversCarouselItemsAction prepareCarouselItemsAction,
        IPodcastLayoutCreator podcastLayoutCreator,
        IContributorLayoutCreator contributorLayoutCreator,
        IPlaylistLayoutCreator playlistLayoutCreator,
        IAlbumLayoutCreator albumLayoutCreator)
    {
        _discoverClient = discoverClient;
        _mediaPlayer = mediaPlayer;
        _trackFactory = trackFactory;
        _bmmLanguageBinder = bmmLanguageBinder;
        _user = user;
        _prepareCarouselItemsAction = prepareCarouselItemsAction;
        _podcastLayoutCreator = podcastLayoutCreator;
        _contributorLayoutCreator = contributorLayoutCreator;
        _playlistLayoutCreator = playlistLayoutCreator;
        _albumLayoutCreator = albumLayoutCreator;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        var discoverItems = (await _discoverClient.GetDocumentsCarPlay(AppTheme.Light, CachePolicy.UseCacheAndRefreshOutdated)).ToList();
        
        var grouped = new List<GroupedDocuments>();
        GroupedDocuments? currentGroup = null;

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
            trackListItems.AddRange(await GetTrackListItems(cpInterfaceController, group.Documents));
            sections.Add(new CPListSection(trackListItems.ToArray(), group.Title, null));
        }
        
        var homeTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.MenuViewModel_Home], sections.ToArray());
        homeTemplate.TabTitle = _bmmLanguageBinder[Translations.MenuViewModel_Home];
        homeTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconHome.ToNameWithExtension());
        return homeTemplate;
    }

    private async Task<IList<ICPListTemplateItem>> GetTrackListItems(CPInterfaceController cpInterfaceController, IEnumerable<Document> documents)
    {
        return await Task.WhenAll(documents
            .Select(async d =>
            {
                CPListItem trackListItem = null;

                switch (d)
                {
                    case ContinueListeningTile continueListeningTile:
                    {
                        trackListItem = await CreateItemForTile(cpInterfaceController, continueListeningTile);
                        break;
                    }
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
                    case Contributor contributor:
                    {
                        var coverImage = await contributor.Cover.ToUIImage();
                        trackListItem = new CPListItem(contributor.Name, null, coverImage);
                        trackListItem.Handler = async (item, block) =>
                        {
                            var contributorLayout = await _contributorLayoutCreator.Create(cpInterfaceController, contributor.Id, contributor.Name);
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
        
    private async Task<CPListItem> CreateItemForTile(
        CPInterfaceController cpInterfaceController,
        ContinueListeningTile continueListeningTile)
    {
        var dateTimeToPodcastPublishDateLabelValueConverter = new DateTimeToPodcastPublishDateLabelValueConverter();
        var dateTimeToPodcastPublishDayOfWeekLabelValueConverter = new DateTimeToPodcastPublishDayOfWeekLabelValueConverter();
        
        var image = await continueListeningTile.CoverUrl.ToUIImage();
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

        var item = new CPListItem(continueListeningTile.Label, subtitle.ToString(), image);
        item.Handler = async (listItem, block) =>
        {
            await _mediaPlayer.Play(continueListeningTile.Track.EncloseInArray(), continueListeningTile.Track);
            var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
            await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
            block();
        };
        return item;
    }
}