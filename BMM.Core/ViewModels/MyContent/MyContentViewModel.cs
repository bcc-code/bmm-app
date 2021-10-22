using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Messages;
using BMM.Core.Models;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels.MyContent
{
    public class MyContentViewModel : ContentBaseViewModel
    {
        private MvxSubscriptionToken _playlistStateChangedMessageSubscriptionKey;

        public MyContentViewModel(IOfflineTrackCollectionStorage downloader, IStorageManager storageManager)
            : base(downloader, storageManager)
        {
        }

        protected override Task Initialization()
        {
            _playlistStateChangedMessageSubscriptionKey =
                Messenger.Subscribe<PlaylistStateChangedMessage>(m => ReloadCommand.ExecuteAsync());
            return base.Initialization();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            Messenger.Unsubscribe<PlaylistStateChangedMessage>(_playlistStateChangedMessageSubscriptionKey);
            base.ViewDestroy(viewFinishing);
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollectionsExceptMyTracks = await base.LoadItems(policy);
            return PrepareGroupedDocuments(allCollectionsExceptMyTracks);
        }

        private IEnumerable<Document> PrepareGroupedDocuments(IEnumerable<Document> documentsCollections)
        {
            var listOfDocuments = new List<Document>();

            var trackCollectionsList = documentsCollections
                .OfType<TrackCollection>()
                .ToList();

            var sharedWithMe = trackCollectionsList
                .Where(t => !t.CanEdit)
                .ToList();

            var trackCollectionList = trackCollectionsList
                .Except(sharedWithMe)
                .ToList();

            var myPlaylistHeader = GetHeader(Translations.MyContentViewModel_MyPlaylists);

            listOfDocuments.Add(myPlaylistHeader);
            listOfDocuments.AddRange(BuildPinnedItems());
            listOfDocuments.AddRange(trackCollectionList);

            if (!sharedWithMe.Any())
                return listOfDocuments;

            var sharedWithMeHeader = GetHeader(Translations.MyContentViewModel_SharedWithMe);

            listOfDocuments.Add(sharedWithMeHeader);
            listOfDocuments.AddRange(sharedWithMe);

            return listOfDocuments;
        }

        private ChapterHeader GetHeader(string titleKey)
        {
            return new ChapterHeader
            {
                DocumentType = DocumentType.ChapterHeader,
                Title = TextSource[titleKey]
            };
        }

        private IEnumerable<PinnedItem> BuildPinnedItems()
        {
            return new List<PinnedItem>
            {
                new PinnedItem()
                {
                    Title = TextSource[Translations.MyContentViewModel_DownloadedContent],
                    Action = new MvxAsyncCommand<PinnedItem>(async execute => await NavigationService.Navigate<DownloadedContentViewModel>()),
                    Icon = "icon_download"
                },
                new PinnedItem()
                {
                    Title = TextSource[Translations.MyContentViewModel_FollowedPodcasts],
                    Action = new MvxAsyncCommand<PinnedItem>(async execute => await NavigationService.Navigate<FollowedPodcastsViewModel>()),
                    Icon = "icon_podcast"
                }
            };
        }
    }
}