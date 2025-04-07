using System.Diagnostics.CodeAnalysis;
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
    private readonly IFirebaseRemoteConfig _config;
    private readonly IUserStorage _user;
    private readonly IPrepareCoversCarouselItemsAction _prepareCarouselItemsAction;

    public HomeLayoutCreator(
        IDiscoverClient discoverClient,
        IMediaPlayer mediaPlayer,
        ITrackPOFactory trackFactory,
        IBMMLanguageBinder bmmLanguageBinder,
        IFirebaseRemoteConfig config,
        IUserStorage user,
        IPrepareCoversCarouselItemsAction prepareCarouselItemsAction)
    {
        _discoverClient = discoverClient;
        _mediaPlayer = mediaPlayer;
        _trackFactory = trackFactory;
        _bmmLanguageBinder = bmmLanguageBinder;
        _config = config;
        _user = user;
        _prepareCarouselItemsAction = prepareCarouselItemsAction;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        int? age = _config.SendAgeToDiscover ? _user.GetUser().Age : null;
        var exploreNewest = (await _discoverClient.GetDocuments(age, AppTheme.Light, CachePolicy.UseCacheAndRefreshOutdated)).ToList();
        
        var tiles = exploreNewest
            .OfType<ContinueListeningTile>()
            .ToList();

        var contributors = exploreNewest
            .OfType<Contributor>()
            .ToList();

        var exploreImages = new List<UIImage>();
        
        foreach (var tile in tiles)
            exploreImages.Add(await ImageService.Instance.LoadUrl(tile.CoverUrl).AsUIImageAsync());
        
        var exploreImageRowItem = new CPListImageRowItem(
            _bmmLanguageBinder[Translations.ExploreViewModel_Title],
            exploreImages.ToArray(),
            tiles.Select(x => x.Title).ToArray());
        
        exploreImageRowItem.Handler = async (item, block) =>
        {
            Console.WriteLine("Item 2 tapped");
            await Task.Delay(2000);
            block();
        };

        exploreImageRowItem.ListImageRowHandler = async (item, index, block) =>
        {
            block();
        };
        
        var contributorsImages = new List<UIImage>();
        
        foreach (var tile in contributors)
            contributorsImages.Add(await ImageService.Instance.LoadUrl(tile.Cover).AsUIImageAsync());
        
        var contributorsImageRowItem = new CPListImageRowItem(
            _bmmLanguageBinder[Translations.ExploreViewModel_Title],
            contributorsImages.ToArray(),
            contributors.Select(x => x.Name).ToArray());

        contributorsImageRowItem.Handler = async (item, block) =>
        {
            block();
        };
        
        exploreImageRowItem.ListImageRowHandler = async (item, index, block) =>
        {
            block();
        };

        var homeSection = new CPListSection(((ICPListTemplateItem)exploreImageRowItem).EncloseInArray());
        var contributorsSection = new CPListSection(((ICPListTemplateItem)contributorsImageRowItem).EncloseInArray());
        
        var homeTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.MenuViewModel_Home], [homeSection, contributorsSection]);
        homeTemplate.TabTitle = _bmmLanguageBinder[Translations.MenuViewModel_Home];
        homeTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconHome.ToNameWithExtension());
        return homeTemplate;
    }
}