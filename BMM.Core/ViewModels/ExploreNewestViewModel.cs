using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.GuardedActions.Navigation.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Factories.Streak;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.PlayObserver.Streak;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.POs.Base;
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
        private readonly ITranslateDocsAction _translateDocsAction;
        private readonly IAppLanguageProvider _appLanguageProvider;
        private readonly IUserStorage _user;
        private readonly IFirebaseRemoteConfig _config;
        private readonly IListeningStreakPOFactory _listeningStreakPOFactory;
        private readonly MvxSubscriptionToken _listeningStreakChangedMessageToken;
        private readonly MvxSubscriptionToken _playbackStatusChangedMessageToken;

        public ExploreNewestViewModel(
            IStreakObserver streakObserver,
            ISettingsStorage settings,
            INavigateToViewModelAction navigateToViewModelAction,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            IPrepareTileCarouselItemsAction prepareTileCarouselItemsAction,
            ITranslateDocsAction translateDocsAction,
            IAppLanguageProvider appLanguageProvider,
            IUserStorage user,
            IFirebaseRemoteConfig config,
            IListeningStreakPOFactory listeningStreakPOFactory)
        {
            _streakObserver = streakObserver;
            _settings = settings;
            _navigateToViewModelAction = navigateToViewModelAction;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _prepareTileCarouselItemsAction = prepareTileCarouselItemsAction;
            _translateDocsAction = translateDocsAction;
            _appLanguageProvider = appLanguageProvider;
            _user = user;
            _config = config;
            _listeningStreakPOFactory = listeningStreakPOFactory;
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
            var docs = (await Client.Discover.GetDocuments(_appLanguageProvider.GetAppLanguage(), age, policy)).ToList();
            await _streakObserver.UpdateStreakIfLocalVersionIsNewer(docs);
            bool hideStreak = await _settings.GetStreakHidden();
            var filteredDocs = HideStreakInList(hideStreak, HideTeaserPodcastsInList(docs));
            var translatedDocs = await _translateDocsAction.ExecuteGuarded(filteredDocs);
            var docsWithCoversCarousel = await _prepareCoversCarouselItemsAction.ExecuteGuarded(translatedDocs);
            var presentationItems = await _prepareTileCarouselItemsAction.ExecuteGuarded(docsWithCoversCarousel);
            SetAdditionalElements(presentationItems);
            return presentationItems;
        }

        private void SetAdditionalElements(IList<IDocumentPO> translatedDocs)
        {
            translatedDocs.Insert(0, new SimpleMarginPO());
            foreach (var discoverSectionHeader in translatedDocs.OfType<DiscoverSectionHeaderPO>())
                discoverSectionHeader.Origin = PlaybackOriginString;
        }

        protected override Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            if (item is not TrackPO)
                return base.DocumentAction(item, list);

            var tracksInSameSection = GetAdjacentTracksInSameSection(item);

            return base.DocumentAction(item, tracksInSameSection);
        }

        protected override Task OptionsAction(Document item)
        {
            if (item is not YearInReviewTeaser)
                return base.OptionsAction(item);

            var yearInReviewItem = Documents.First(x => x.DocumentType == DocumentType.YearInReview);
            Documents.RefreshItem(yearInReviewItem);
            return Task.CompletedTask;
        }

        private IList<Document> HideStreakInList(bool hideStreak, IList<Document> documents)
        {
            if (hideStreak)
                return documents.Where(d => d.DocumentType != DocumentType.ListeningStreak).ToList();
            return documents;
        }

        private IList<Document> HideTeaserPodcastsInList(IList<Document> documents)
        {
            var unwantedDocs = documents
                .TakeWhile(d => d.DocumentType != DocumentType.DiscoverSectionHeader)
                .Where(d => d.DocumentType == DocumentType.Podcast)
                .Where(d => d.Id == FraKaareConstants.FraKårePodcastId || d.Id == AslaksenConstants.AslaksenPodcastId);
            return documents.Except(unwantedDocs).ToList();
        }

        private List<Track> GetAdjacentTracksInSameSection(IDocumentPO item)
        {
            int index = Documents.IndexOf(item);

            if (index == -1 && item is TrackPO trackToPlayPO)
                return trackToPlayPO.Track.EncloseInArray().ToList();
            
            var tracks = new List<Track>();
            int previousCounter = 1;
            while (index - previousCounter > 0 && Documents[index - previousCounter] is TrackPO trackPO)
            {
                tracks.Add(trackPO.Track);
                previousCounter++;
            }

            int nextCounter = 0;
            while (Documents.Count > index + nextCounter && Documents[index + nextCounter] is TrackPO trackPO)
            {
                tracks.Add(trackPO.Track);
                nextCounter++;
            }

            return tracks;
        }
    }
}