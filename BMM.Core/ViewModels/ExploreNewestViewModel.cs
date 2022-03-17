using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.GuardedActions.Navigation.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.PlayObserver.Streak;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Messages;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross;
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
        private readonly IPrepareContinueListeningCarouselItemsAction _prepareContinueListeningCarouselItemsAction;
        private readonly ITranslateDocsAction _translateDocsAction;
        private readonly IAppLanguageProvider _appLanguageProvider;
        private readonly IUserStorage _user;
        private readonly IFirebaseRemoteConfig _config;

        public AslaksenTeaserViewModel AslaksenTeaserViewModel { get; private set; }

        public ExploreRadioViewModel RadioViewModel { get; private set; }

        private MvxSubscriptionToken _listeningStreakToken;
        private ExceptionHandlingCommand<ContinueListeningTile> _continuePlayingCommand;
        private ExceptionHandlingCommand<ContinueListeningTile> _tileClickedCommand;
        private ExceptionHandlingCommand<ContinueListeningTile> _shuffleButtonCommand;

        public ExploreNewestViewModel(
            IStreakObserver streakObserver,
            IMvxMessenger messenger,
            ISettingsStorage settings,
            INavigateToViewModelAction navigateToViewModelAction,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            IPrepareContinueListeningCarouselItemsAction prepareContinueListeningCarouselItemsAction,
            ITranslateDocsAction translateDocsAction,
            IAppLanguageProvider appLanguageProvider,
            IUserStorage user,
            IFirebaseRemoteConfig config,
            IContinuePlayingAction continuePlayingAction,
            ITileClickedAction tileClickedAction,
            IShuffleButtonClickedAction shuffleButtonClickedAction)
        {
            _streakObserver = streakObserver;
            _settings = settings;
            _navigateToViewModelAction = navigateToViewModelAction;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _prepareContinueListeningCarouselItemsAction = prepareContinueListeningCarouselItemsAction;
            _translateDocsAction = translateDocsAction;
            _appLanguageProvider = appLanguageProvider;
            _user = user;
            _config = config;
            ContinuePlayingCommand = continuePlayingAction.Command;
            TileClickedCommand = tileClickedAction.Command;
            ShuffleButtonCommand = shuffleButtonClickedAction.Command;
            AslaksenTeaserViewModel = Mvx.IoCProvider.IoCConstruct<AslaksenTeaserViewModel>();
            RadioViewModel = Mvx.IoCProvider.IoCConstruct<ExploreRadioViewModel>();
            _listeningStreakToken = messenger.Subscribe<ListeningStreakChangedMessage>(ListeningStreakChanged);
            TrackInfoProvider = new TypeKnownTrackInfoProvider();
        }

        public IMvxAsyncCommand<Type> NavigateToViewModelCommand => _navigateToViewModelAction.Command;
        public IMvxAsyncCommand<ContinueListeningTile> ContinuePlayingCommand { get; }
        public IMvxAsyncCommand<ContinueListeningTile> TileClickedCommand { get; }
        public IMvxAsyncCommand<ContinueListeningTile> ShuffleButtonCommand { get; }
        
        private void ListeningStreakChanged(ListeningStreakChangedMessage message)
        {
            var index = Documents.FindIndex(document => document.DocumentType == DocumentType.ListeningStreak);
            if (index >= 0)
                Documents.ReplaceRange(new[] {message.ListeningStreak}, index, 1);
        }

        protected override Task Initialization()
        {
            Mvx.IoCProvider.Resolve<INotificationCenter>().AppLanguageChanged += (sender, e) => UpdatePodcastName();
            UpdatePodcastName();

            return Task.WhenAll(
                AslaksenTeaserViewModel.Initialize(),
                RadioViewModel.Initialize(),
                base.Initialization()
            );
        }

        public override Task Refresh()
        {
            return Task.WhenAll(
                AslaksenTeaserViewModel.Refresh(),
                RadioViewModel.Refresh(),
                base.Refresh()
            );
        }

        public override Task Load()
        {
            return Task.WhenAll(
                base.Load()
            );
        }

        private void UpdatePodcastName()
        {
            AslaksenTeaserViewModel.Podcast.Title = TextSource[Translations.ExploreNewestViewModel_AslaksenTeaserHeader];
        }

        public override CacheKeys? CacheKey => CacheKeys.DiscoverGetDocuments;

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var age = _config.SendAgeToDiscover ? _user.GetUser().Age : null;
            var docs = (await Client.Discover.GetDocuments(_appLanguageProvider.GetAppLanguage(), age, policy)).ToList();
            await _streakObserver.UpdateStreakIfLocalVersionIsNewer(docs);
            bool hideStreak = await _settings.GetStreakHidden();
            HideTeasers(docs);
            var filteredDocs = HideStreakInList(hideStreak, HideTeaserPodcastsInList(docs));
            var translatedDocs = await _translateDocsAction.ExecuteGuarded(filteredDocs);
            AddAdditionalElements(translatedDocs);
            var docsWithCoversCarousel = await _prepareCoversCarouselItemsAction.ExecuteGuarded(translatedDocs);
            return await _prepareContinueListeningCarouselItemsAction.ExecuteGuarded(docsWithCoversCarousel);
        }

        private void AddAdditionalElements(IList<Document> translatedDocs)
        {
            int indexToAddAdditionalElements = translatedDocs.LastIndexOfElementType(typeof(InfoMessage), 1);

            if (AslaksenTeaserViewModel.ShowTeaser)
                translatedDocs.Insert(indexToAddAdditionalElements, new AslaksenTeaser());

            if (RadioViewModel.ShowBmmLive)
                translatedDocs.Insert(indexToAddAdditionalElements, new LiveRadio());
            
            translatedDocs.Insert(0, new SimpleMargin());
        }

        public override void RefreshInBackground()
        {
            base.RefreshInBackground();
            AslaksenTeaserViewModel.RefreshInBackground();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            RadioViewModel.ViewDestroy(viewFinishing);
            base.ViewDestroy(viewFinishing);
        }

        protected override Task DocumentAction(Document item, IList<Track> list)
        {
            if (!(item is Track))
                return base.DocumentAction(item, list);

            var tracksInSameSection = GetAdjacentTracksInSameSection(item);

            return base.DocumentAction(item, tracksInSameSection);
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
                .Where(d => d.Id == FraKaareTeaserViewModel.FraKårePodcastId || d.Id == AslaksenTeaserViewModel.AslaksenPodcastId);
            return documents.Except(unwantedDocs).ToList();
        }

        private void HideTeasers(IList<Document> documents)
        {
            var documentsBeforeFirstHeader = documents
                .TakeWhile(d => d.DocumentType != DocumentType.DiscoverSectionHeader)
                .ToList();
            AslaksenTeaserViewModel.ShowTeaser = documentsBeforeFirstHeader.Any(d =>
                d.DocumentType == DocumentType.Podcast && d.Id == AslaksenTeaserViewModel.AslaksenPodcastId);
        }

        private List<Track> GetAdjacentTracksInSameSection(Document item)
        {
            int index = Documents.IndexOf(item);

            if (index == -1 && item is Track trackToPlay)
                return trackToPlay.EncloseInArray().ToList();
            
            var tracks = new List<Track>();
            int previousCounter = 1;
            while (index - previousCounter > 0 && Documents[index - previousCounter] is Track track)
            {
                tracks.Add(track);
                previousCounter++;
            }

            int nextCounter = 0;
            while (Documents.Count > index + nextCounter && Documents[index + nextCounter] is Track track)
            {
                tracks.Add(track);
                nextCounter++;
            }

            return tracks;
        }
    }
}