using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.Localization.Interfaces;
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
    private readonly IPodcastClient _podcastClient;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly ITrackPOFactory _trackFactory;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;

    public HomeLayoutCreator(
        IPodcastClient podcastClient,
        IMediaPlayer mediaPlayer,
        ITrackPOFactory trackFactory,
        IBMMLanguageBinder bmmLanguageBinder)
    {
        _podcastClient = podcastClient;
        _mediaPlayer = mediaPlayer;
        _trackFactory = trackFactory;
        _bmmLanguageBinder = bmmLanguageBinder;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        var podcasts = (await _podcastClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated)).ToList();

        var images = new List<UIImage>();
        foreach (var item in podcasts)
            images.Add(await ImageService.Instance.LoadUrl(item.Cover).AsUIImageAsync());

        var podcastsImageRowItem = new CPListImageRowItem(_bmmLanguageBinder[Translations.PodcastsViewModel_Title], images.ToArray(), podcasts.Select(x => x.Title).ToArray());
        podcastsImageRowItem.Handler = async (item, block) =>
        {
            Console.WriteLine("Item 2 tapped");
            await Task.Delay(2000);
            block();
        };

        podcastsImageRowItem.ListImageRowHandler = async (item, index, block) =>
        {
            var loadingItem = new CPListItem(text: "Loading...", detailText: null);
            var section = new CPListSection(loadingItem.EncloseInArray().OfType<ICPListTemplateItem>().ToArray());
            var template = new CPListTemplate(title: podcasts.First().Title, sections: [section]);

            await cpInterfaceController.PushTemplateAsync(template, true);
            var tracks = await _podcastClient.GetTracks(podcasts.First().Id, CachePolicy.UseCacheAndRefreshOutdated);
            var tracksPO = tracks.Select(x => _trackFactory.Create(
                new AudiobookPodcastInfoProvider(new DefaultTrackInfoProvider()),
                null,
                x));

            var tracksListItem = tracksPO.Select(x =>
            {
                var trackListitem = new CPListItem(x.TrackTitle, $"{x.TrackSubtitle} {x.TrackMeta}");
                trackListitem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                trackListitem.Handler = async (item, block) =>
                {
                    await _mediaPlayer.Play(tracks.OfType<IMediaTrack>().ToList(), x.Track);
                    var nowPlayingTemplate = CPNowPlayingTemplate.SharedTemplate;
                    await cpInterfaceController.PushTemplateAsync(nowPlayingTemplate, true);
                    block();
                };
                return trackListitem;
            });
            var updatedSection = new CPListSection(tracksListItem.OfType<ICPListTemplateItem>().ToArray());

            template.UpdateSections(updatedSection.EncloseInArray());
            block();
        };

        var homeSection = new CPListSection(podcastsImageRowItem.EncloseInArray().OfType<ICPListTemplateItem>().ToArray());
        var homeTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.MenuViewModel_Home], [homeSection]);
        homeTemplate.TabTitle = _bmmLanguageBinder[Translations.MenuViewModel_Home];
        homeTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconHome.ToNameWithExtension());
        return homeTemplate;
    }
}