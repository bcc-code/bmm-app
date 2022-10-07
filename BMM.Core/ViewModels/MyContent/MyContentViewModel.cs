using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Messages;
using BMM.Core.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
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

        public MyContentViewModel(
            IStorageManager storageManager,
            ITrackCollectionPOFactory trackCollectionPOFactory)
            : base(
                storageManager,
                trackCollectionPOFactory)
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

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollectionsExceptMyTracks = await base.LoadItems(policy);
            return PrepareGroupedDocuments(allCollectionsExceptMyTracks);
        }

        private IEnumerable<IDocumentPO> PrepareGroupedDocuments(IEnumerable<IDocumentPO> documentsCollections)
        {
            var listOfDocuments = new List<IDocumentPO>();

            var trackCollectionsList = documentsCollections
                .OfType<TrackCollectionPO>()
                .ToList();

            var sharedWithMe = trackCollectionsList
                .Where(t => !t.TrackCollection.CanEdit)
                .ToList();

            var trackCollectionList = trackCollectionsList
                .Except(sharedWithMe)
                .ToList();

            var myPlaylistHeader = GetHeader(Translations.MyContentViewModel_MyPlaylists);

            listOfDocuments.Add(new ChapterHeaderPO(myPlaylistHeader));
            listOfDocuments.AddRange(BuildPinnedItems());
            listOfDocuments.AddRange(trackCollectionList);

            if (!sharedWithMe.Any())
                return listOfDocuments;

            var sharedWithMeHeader = GetHeader(Translations.MyContentViewModel_SharedWithMe);

            listOfDocuments.Add(new ChapterHeaderPO(sharedWithMeHeader));
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

        private IEnumerable<PinnedItemPO> BuildPinnedItems()
        {
            return new List<PinnedItem>
            {
                new()
                {
                    Title = TextSource[Translations.MyContentViewModel_DownloadedContent],
                    Action = new MvxAsyncCommand<PinnedItem>(async execute => await NavigationService.Navigate<DownloadedContentViewModel>()),
                    Icon = "icon_download"
                },
                new()
                {
                    Title = TextSource[Translations.MyContentViewModel_FollowedPodcasts],
                    Action = new MvxAsyncCommand<PinnedItem>(async execute => await NavigationService.Navigate<FollowedPodcastsViewModel>()),
                    Icon = "icon_podcast"
                }
            }.Select(pi => new PinnedItemPO(pi));
        }
    }
}