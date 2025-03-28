using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.GuardedActions.Navigation.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DeepLinking;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Factories.Streak;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.PlayObserver.Streak;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.NewMediaPlayer;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class ExploreNewestViewModel : DocumentsViewModel
    {
        private readonly IStreakObserver _streakObserver;
        private readonly ISettingsStorage _settings;
        private readonly INavigateToViewModelAction _navigateToViewModelAction;
        private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;
        private readonly IPrepareTileCarouselItemsAction _prepareTileCarouselItemsAction;
        private readonly IUserStorage _user;
        private readonly IFirebaseRemoteConfig _config;
        private readonly IListeningStreakPOFactory _listeningStreakPOFactory;
        private readonly IAddToQueueAdditionalMusic _addToQueueAdditionalMusic;
        private readonly ICheckAndShowAchievementUnlockedScreenAction _checkAndShowAchievementUnlockedScreenAction;
        private readonly MvxSubscriptionToken _listeningStreakChangedMessageToken;
        private readonly MvxSubscriptionToken _playbackStatusChangedMessageToken;
        private readonly IDeviceInfo _deviceInfo;

        public ExploreNewestViewModel(
            IStreakObserver streakObserver,
            ISettingsStorage settings,
            INavigateToViewModelAction navigateToViewModelAction,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            IPrepareTileCarouselItemsAction prepareTileCarouselItemsAction,
            IUserStorage user,
            IFirebaseRemoteConfig config,
            IListeningStreakPOFactory listeningStreakPOFactory,
            IAddToQueueAdditionalMusic addToQueueAdditionalMusic,
            ICheckAndShowAchievementUnlockedScreenAction checkAndShowAchievementUnlockedScreenAction,
            IDeviceInfo deviceInfo)
        {
            _streakObserver = streakObserver;
            _settings = settings;
            _navigateToViewModelAction = navigateToViewModelAction;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _prepareTileCarouselItemsAction = prepareTileCarouselItemsAction;
            _user = user;
            _config = config;
            _listeningStreakPOFactory = listeningStreakPOFactory;
            _addToQueueAdditionalMusic = addToQueueAdditionalMusic;
            _checkAndShowAchievementUnlockedScreenAction = checkAndShowAchievementUnlockedScreenAction;
            _deviceInfo = deviceInfo;
            _listeningStreakChangedMessageToken = Messenger.Subscribe<ListeningStreakChangedMessage>(ListeningStreakChanged);
            _playbackStatusChangedMessageToken = Messenger.Subscribe<PlaybackStatusChangedMessage>(PlaybackStateChanged);
            _prepareTileCarouselItemsAction.AttachDataContext(this);
            TrackInfoProvider = new TypeKnownTrackInfoProvider();
        }
        
        public IMvxAsyncCommand<Type> NavigateToViewModelCommand => _navigateToViewModelAction.Command;

        private void ListeningStreakChanged(ListeningStreakChangedMessage message)
        {
            var index = Documents.FindIndex(document => document.DocumentType == DocumentType.ListeningStreak);
            
            if (index >= 0)
                Documents.ReplaceRange(new[] { _listeningStreakPOFactory.Create(message.ListeningStreak)}, index, 1);
        }

        private void PlaybackStateChanged(PlaybackStatusChangedMessage playbackStatusChangedMessage)
        {
            if (playbackStatusChangedMessage.PlaybackState.PlayStatus.IsOneOf(PlayStatus.Playing, PlayStatus.Paused, PlayStatus.Stopped))
                RefreshContinueListeningItems();
        }

        private void RefreshContinueListeningItems()
        {
            var itemsToRefresh = Documents.OfType<TileCollectionPO>().ToList();
            itemsToRefresh.ForEach(i => i.RefreshState());
        }

        public override CacheKeys? CacheKey => CacheKeys.DiscoverGetDocuments;

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var age = _config.SendAgeToDiscover ? _user.GetUser().Age : null;
            var docs = (await Client.Discover.GetDocuments(age, await _deviceInfo.GetCurrentTheme() , policy)).ToList();
            await _streakObserver.UpdateStreakIfLocalVersionIsNewer(docs);
            var filteredDocs = await HideElementsInList(docs);
            var docsWithCoversCarousel = await _prepareCoversCarouselItemsAction.ExecuteGuarded(filteredDocs);
            var presentationItems = await _prepareTileCarouselItemsAction.ExecuteGuarded(docsWithCoversCarousel);
            SetAdditionalElements(presentationItems);
            return presentationItems;
        }

        private void SetAdditionalElements(IList<IDocumentPO> translatedDocs)
        {
            translatedDocs.Insert(0, new SimpleMarginPO());
            foreach (var discoverSectionHeader in translatedDocs.OfType<DiscoverSectionHeaderPO>())
                discoverSectionHeader.Origin = PlaybackOriginString();
        }

        protected override async Task Initialization()
        {
            await base.Initialization();
            
            if (_config.ShouldCheckAchievementsAtStart)
                await _checkAndShowAchievementUnlockedScreenAction.ExecuteGuarded();
        }

        protected override async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            if (item is not TrackPO)
            {
                await base.DocumentAction(item, list);
                return;
            }

            var result = GetAdjacentTracksInSameSection(item);
            await base.DocumentAction(item, result.Tracks);
            
            if (result.ShouldLoadAdditionalMusic)
                await _addToQueueAdditionalMusic.ExecuteGuarded(PlaybackOriginString());
        }

        protected override Task OptionsAction(Document item)
        {
            if (item is not YearInReviewTeaser)
                return base.OptionsAction(item);

            var yearInReviewItem = Documents.First(x => x.DocumentType == DocumentType.YearInReview);
            Documents.RefreshItem(yearInReviewItem);
            return Task.CompletedTask;
        }

        private async Task<IList<Document>> HideElementsInList(IList<Document> documents)
        {
            bool hideStreak = await _settings.GetStreakHidden();
            bool bibleStudyEnabled = await _settings.GetBibleStudyOnHomeEnabled();

            return documents
                .Where(d => !hideStreak || d.DocumentType != DocumentType.ListeningStreak)
                .Where(d => bibleStudyEnabled || d.DocumentType.IsNoneOf(
                    DocumentType.ProjectBox,
                    DocumentType.GibraltarProjectBox,
                    DocumentType.HvheProjectBox))
                .ToList();
        }

        private (List<Track> Tracks, bool ShouldLoadAdditionalMusic) GetAdjacentTracksInSameSection(IDocumentPO item)
        {
            int index = Documents.IndexOf(item);

            if (index == -1 && item is TrackPO trackToPlayPO)
                return (trackToPlayPO.Track.EncloseInArray().ToList(), false);

            bool shouldLoadAdditionalMusic = false;
            
            var tracks = new List<Track>();
            int previousCounter = 1;
            
            while (index - previousCounter > 0)
            {
                var previousItem = Documents[index - previousCounter];
                
                if (previousItem is TrackPO trackPO)
                {
                    tracks.Add(trackPO.Track);
                    previousCounter++;
                    continue;
                }

                // if the user plays the music from the explore page, we want to add more item to the queue
                // therefore we need to determine, if the opened item comes from music section
                // and we can do it by checking if the nearest header contains specific link
                if (previousItem is DiscoverSectionHeaderPO discoverSectionHeaderPO
                    && discoverSectionHeaderPO.DiscoverSectionHeader.Link.EndsWith(DeepLinkHandler.Music))
                {
                    shouldLoadAdditionalMusic = true;
                }
                
                break;
            }

            int nextCounter = 0;
            while (Documents.Count > index + nextCounter && Documents[index + nextCounter] is TrackPO trackPO)
            {
                tracks.Add(trackPO.Track);
                nextCounter++;
            }

            return (tracks, shouldLoadAdditionalMusic);
        }
    }
}