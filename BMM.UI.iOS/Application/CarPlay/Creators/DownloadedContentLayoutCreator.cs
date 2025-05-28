using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ValueConverters.TrackCollections;
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
public class DownloadedContentLayoutCreator : BaseLayoutCreator, IDownloadedContentLayoutCreator
{
    private CPInterfaceController _cpInterfaceController;
    private CPListTemplate _downloadedListTemplate;
    private ITrackCollectionClient TrackCollectionClient => Mvx.IoCProvider!.Resolve<ITrackCollectionClient>();
    private ITrackCollectionPOFactory TrackCollectionPOFactory => Mvx.IoCProvider!.Resolve<ITrackCollectionPOFactory>();
    private IPrepareDownloadedContentItemsAction PrepareDownloadedContentItemsAction => Mvx.IoCProvider!.Resolve<IPrepareDownloadedContentItemsAction>();
    private IFollowedPodcastsContentLayoutCreator FollowedPodcastsContentLayoutCreator => Mvx.IoCProvider!.Resolve<IFollowedPodcastsContentLayoutCreator>();
    private IBMMLanguageBinder BMMLanguageBinder => Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>();
    private ITrackCollectionContentLayoutCreator TrackCollectionContentLayoutCreator => Mvx.IoCProvider!.Resolve<ITrackCollectionContentLayoutCreator>();
    private IPlaylistLayoutCreator PlaylistLayoutCreator => Mvx.IoCProvider!.Resolve<IPlaylistLayoutCreator>();

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        _cpInterfaceController = cpInterfaceController;
        _downloadedListTemplate =
            new CPListTemplate(BMMLanguageBinder[Translations.DownloadedContentViewModel_Title], LoadingSection.Create());
        SafeLoad().FireAndForget();
        return _downloadedListTemplate;
    }

    public override async Task Load()
    {
        var allCollections = await TrackCollectionClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);

        var items = allCollections
            .OrderByDescending(c => c.Id)
            .Select(tc => TrackCollectionPOFactory.Create(tc))
            .ToList();
        
        var downloadedContentItems = await PrepareDownloadedContentItemsAction.ExecuteGuarded(items);
        var converter = new TrackCollectionToListViewItemSubtitleLabelConverter();

        var covers = await downloadedContentItems.DownloadCovers();
        
        var tracklistItems = await Task.WhenAll(downloadedContentItems
            .Select(async x =>
            {
                CPListItem trackListItem = null;

                switch (x)
                {
                    case PinnedItemPO pinnedItemPO:
                    {
                        var image = UIImage.FromBundle(ImageResourceNames.IconFollowedPodcastsCarplay.ToIosImageName());
                        trackListItem = new CPListItem(pinnedItemPO.PinnedItem.Title, null, image);
                        trackListItem.Handler = async (item, block) =>
                        {
                            if (pinnedItemPO.PinnedItem.ActionType == PinnedItemActionType.DownloadedFollowedPodcasts)
                            {
                                var followedPodcastsContentLayout =
                                    await FollowedPodcastsContentLayoutCreator.Create(CpInterfaceController);
                                await CpInterfaceController.PushTemplateAsync(followedPodcastsContentLayout, true);
                            }

                            block();
                        };

                        break;
                    }
                    case TrackCollectionPO trackCollectionPO:
                    {
                        string detailsText = (string)converter.Convert(
                            trackCollectionPO.TrackCollection,
                            typeof(string),
                            null,
                            CultureInfo.CurrentUICulture);

                        trackListItem = new CPListItem(trackCollectionPO.TrackCollection.Name, detailsText);
                        trackListItem.Handler = async (item, block) =>
                        {
                            var trackCollectionContentLayout = await TrackCollectionContentLayoutCreator.Create(
                                CpInterfaceController,
                                trackCollectionPO.TrackCollection.Name,
                                trackCollectionPO.Id);
                            await CpInterfaceController.PushTemplateAsync(trackCollectionContentLayout, true);
                            block();
                        };

                        break;
                    }
                    case PlaylistPO playlistPO:
                    {
                        trackListItem = new CPListItem(playlistPO.Title, null, covers.GetCover(playlistPO.Cover));
                        trackListItem.Handler = async (item, block) =>
                        {
                            var playlistLayout =
                                await PlaylistLayoutCreator.Create(CpInterfaceController, playlistPO.Id, playlistPO.Title);
                            await CpInterfaceController.PushTemplateAsync(playlistLayout, true);
                            block();
                        };

                        break;
                    }
                    case AlbumPO albumPO:
                    {
                        trackListItem = new CPListItem(albumPO.Title, null, covers.GetCover(albumPO.Cover));
                        trackListItem.Handler = async (item, block) => { block(); };

                        break;
                    }
                }

                trackListItem.AccessoryType = CPListItemAccessoryType.DisclosureIndicator;
                return (ICPListTemplateItem)trackListItem;
            }));

        var section = new CPListSection(tracklistItems);
        _downloadedListTemplate.SafeUpdateSections(section.EncloseInArray());
    }
}