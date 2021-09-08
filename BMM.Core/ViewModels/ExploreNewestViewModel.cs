using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.PlayObserver.Streak;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Messages;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class ExploreNewestViewModel : DocumentsViewModel
    {
        private readonly IStreakObserver _streakObserver;
        private readonly ISettingsStorage _settings;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public FraKaareTeaserViewModel FraKaareTeaserViewModel { get; private set; }

        public AslaksenTeaserViewModel AslaksenTeaserViewModel { get; private set; }

        public ExploreRadioViewModel RadioViewModel { get; private set; }

        private MvxSubscriptionToken _listeningStreakToken;

        public ExploreNewestViewModel(
            IStreakObserver streakObserver,
            IMvxMessenger messenger,
            ISettingsStorage settings,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _streakObserver = streakObserver;
            _settings = settings;
            _bmmLanguageBinder = bmmLanguageBinder;
            FraKaareTeaserViewModel = Mvx.IoCProvider.IoCConstruct<FraKaareTeaserViewModel>();
            AslaksenTeaserViewModel = Mvx.IoCProvider.IoCConstruct<AslaksenTeaserViewModel>();
            RadioViewModel = Mvx.IoCProvider.IoCConstruct<ExploreRadioViewModel>();
            _listeningStreakToken = messenger.Subscribe<ListeningStreakChangedMessage>(ListeningStreakChanged);
            TrackInfoProvider = new TypeKnownTrackInfoProvider();
        }

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
            TranslateDocs(filteredDocs);
            PrepareCoversCarouselItems(filteredDocs);
            return filteredDocs;
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
            AslaksenTeaserViewModel.ShowTeaser = documentsBeforeFirstHeader.Any(d => d.DocumentType == DocumentType.Podcast && d.Id == AslaksenTeaserViewModel.AslaksenPodcastId);
            FraKaareTeaserViewModel.ShowTeaser = documentsBeforeFirstHeader.Any(d => d.DocumentType == DocumentType.Podcast && d.Id == FraKaareTeaserViewModel.FraKårePodcastId);
        }

        private void TranslateDocs(IList<Document> documents)
        {
            foreach (var document in documents)
            {
                if (!(document is DiscoverSectionHeader sectionHeader))
                    continue;

                if (sectionHeader.TranslationParent == null || sectionHeader.TranslationId == null)
                    continue;

                sectionHeader.Title = GetText(sectionHeader.TranslationParent, sectionHeader.TranslationId);
            }
        }

        private void PrepareCoversCarouselItems(IList<Document> filteredDocs)
        {
            var carouselHeaders = filteredDocs
                .OfType<DiscoverSectionHeader>()
                .Where(d => d.UseCoverCarousel)
                .ToList();

            foreach (var carouselHeader in carouselHeaders)
            {
                var coverDocuments = new List<CoverDocument>();
                int currentIndex = filteredDocs.IndexOf(carouselHeader) + 1;

                while (true)
                {
                    if (currentIndex >= filteredDocs.Count)
                        break;

                    if (!(filteredDocs[currentIndex] is CoverDocument element))
                        break;

                    coverDocuments.Add(element);
                    filteredDocs.RemoveAt(currentIndex);
                }

                if (coverDocuments.Any())
                    filteredDocs.Insert(currentIndex, new CoverCarouselCollection(new ObservableCollection<Document>(coverDocuments)));
            }
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

        private string GetText(string viewModelName, string entryKey)
        {
            return new MvxLanguageBinder(GlobalConstants.GeneralNamespace,
                viewModelName).GetText(entryKey);
        }
    }
}