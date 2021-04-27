using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;

namespace BMM.Core.ViewModels.MyContent
{
    public class MyContentViewModel : ContentBaseViewModel
    {
        // This TextSource is needed because the TrackCollectionsAddToViewModel reuses the fragment axml. Otherwise we would need to duplicate the translations.
        // ReSharper disable once UnusedMember.Global
        public IMvxLanguageBinder MyContentTextSource =>
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, typeof(MyContentViewModel).Name);

        public MyContentViewModel(IOfflineTrackCollectionStorage downloader, IStorageManager storageManager)
            : base(downloader, storageManager)
        {
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollectionsExceptMyTracks = await base.LoadItems(policy);

            List<Document> allCollectionsExceptMyTracksPlusPinnedItems = new List<Document>();
            allCollectionsExceptMyTracksPlusPinnedItems.AddRange(BuildPinnedItems());
            allCollectionsExceptMyTracksPlusPinnedItems.AddRange(allCollectionsExceptMyTracks);

            return allCollectionsExceptMyTracksPlusPinnedItems;
        }

        private IEnumerable<PinnedItem> BuildPinnedItems()
        {
            return new List<PinnedItem>
            {
                new PinnedItem
                {
                    Title = MyContentTextSource.GetText("DownloadedContent"),
                    Action = new MvxAsyncCommand<PinnedItem>(async execute => await _navigationService.Navigate<DownloadedContentViewModel>()),
                    Icon = "icon_download"
                },
                new PinnedItem
                {
                    Title = MyContentTextSource.GetText("FollowedPodcasts"),
                    Action = new MvxAsyncCommand<PinnedItem>(async execute => await _navigationService.Navigate<FollowedPodcastsViewModel>()),
                    Icon = "icon_podcast"
                },
                new PinnedItem
                {
                    Title = MyContentTextSource.GetText("MyTracks"),
                    Action = new MvxAsyncCommand<PinnedItem>(async execute => await _navigationService.Navigate<MyTracksViewModel>()),
                    Icon = "icon_favorites_active_no_fill"
                }
            };
        }
    }
}