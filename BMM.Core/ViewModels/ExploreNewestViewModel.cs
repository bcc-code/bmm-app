using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.GuardedActions.Navigation.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.PlayObserver.Streak;
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
        private readonly ITranslateDocsAction _translateDocsAction;

        public FraKaareTeaserViewModel FraKaareTeaserViewModel { get; private set; }

        public AslaksenTeaserViewModel AslaksenTeaserViewModel { get; private set; }

        public ExploreRadioViewModel RadioViewModel { get; private set; }

        private MvxSubscriptionToken _listeningStreakToken;

        public ExploreNewestViewModel(
            IStreakObserver streakObserver,
            IMvxMessenger messenger,
            ISettingsStorage settings,
            INavigateToViewModelAction navigateToViewModelAction,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            ITranslateDocsAction translateDocsAction)
        {
            _streakObserver = streakObserver;
            _settings = settings;
            _navigateToViewModelAction = navigateToViewModelAction;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _translateDocsAction = translateDocsAction;
            FraKaareTeaserViewModel = Mvx.IoCProvider.IoCConstruct<FraKaareTeaserViewModel>();
            AslaksenTeaserViewModel = Mvx.IoCProvider.IoCConstruct<AslaksenTeaserViewModel>();
            RadioViewModel = Mvx.IoCProvider.IoCConstruct<ExploreRadioViewModel>();
            _listeningStreakToken = messenger.Subscribe<ListeningStreakChangedMessage>(ListeningStreakChanged);
            TrackInfoProvider = new TypeKnownTrackInfoProvider();
        }

        public IMvxAsyncCommand<Type> NavigateToViewModelCommand => _navigateToViewModelAction.Command;

        private void ListeningStreakChanged(ListeningStreakChangedMessage message)
        {
            var index = this.Documents.FindIndex(document => document.DocumentType == DocumentType.ListeningStreak);
            if (index >= 0)
                Documents.ReplaceRange(new[] {message.ListeningStreak}, index, 1);
        }

        protected override Task Initialization()
        {
            Mvx.IoCProvider.Resolve<INotificationCenter>().AppLanguageChanged += (sender, e) => UpdatePodcastName();
            UpdatePodcastName();

            return Task.WhenAll(
                FraKaareTeaserViewModel.Initialize(),
                AslaksenTeaserViewModel.Initialize(),
                RadioViewModel.Initialize(),
                base.Initialization()
            );
        }

        public override Task Refresh()
        {
            return Task.WhenAll(
                FraKaareTeaserViewModel.Refresh(),
                AslaksenTeaserViewModel.Refresh(),
                RadioViewModel.Refresh(),
                base.Refresh()
            );
        }

        public override Task Load()
        {
            return Task.WhenAll(
                FraKaareTeaserViewModel.Load(),
                base.Load()
            );
        }

        private void UpdatePodcastName()
        {
            FraKaareTeaserViewModel.Podcast.Title = TextSource[Translations.ExploreNewestViewModel_FraKaareHeader];
            AslaksenTeaserViewModel.Podcast.Title = TextSource[Translations.ExploreNewestViewModel_AslaksenTeaserHeader];
        }

        public override CacheKeys? CacheKey => CacheKeys.DiscoverGetDocuments;

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var docs = (await Client.Discover.GetDocuments(policy)).ToList();
            await _streakObserver.UpdateStreakIfLocalVersionIsNewer(docs);
            var hideStreak = await _settings.GetStreakHidden();
            HideTeasers(docs);
            var filteredDocs = HideStreakInList(hideStreak, HideTeaserPodcastsInList(docs));
            var translatedDocs = await _translateDocsAction.ExecuteGuarded(filteredDocs);
            return await _prepareCoversCarouselItemsAction.ExecuteGuarded(translatedDocs);
        }

        public override void RefreshInBackground()
        {
            base.RefreshInBackground();
            FraKaareTeaserViewModel.RefreshInBackground();
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
            FraKaareTeaserViewModel.ShowTeaser = documentsBeforeFirstHeader.Any(d =>
                d.DocumentType == DocumentType.Podcast && d.Id == FraKaareTeaserViewModel.FraKårePodcastId);
        }

        private List<Track> GetAdjacentTracksInSameSection(Document item)
        {
            var index = Documents.IndexOf(item);
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