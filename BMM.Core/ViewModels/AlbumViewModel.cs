using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using System.Collections.Specialized;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class AlbumViewModel : DownloadViewModel, IMvxViewModel<int>, IMvxViewModel<Album>, IAlbumViewModel
    {
        private readonly IPlayOrResumePlayAction _playOrResumePlayAction;
        private readonly IDocumentsPOFactory _documentsPOFactory;
        private int _id;

        /// <summary>
        /// While iOS reuses <see cref="BaseViewModel.OptionsAction"/> Android creates a new menu using these commands.
        /// </summary>
        public IMvxCommand AddToPlaylistCommand { get; }

        public IMvxCommand ShareCommand { get; }

        public override IMvxCommand PlayCommand => _playOrResumePlayAction.Command;

        private Album _album;

        public Album Album
        {
            get => _album;
            set
            {
                SetProperty(ref _album, value);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
                RaisePropertyChanged(() => Image);
                RaisePropertyChanged(() => ShowImage);
                RaisePropertyChanged(() => PlayButtonText);
            }
        }

        public override IEnumerable<string> PlaybackOrigin()
        {
            return new[] { Album.Id.ToString() };
        }

        public AlbumViewModel(
            IShareLink shareLink,
            IPlayOrResumePlayAction playOrResumePlayAction,
            IDocumentsPOFactory documentsPOFactory,
            IStorageManager storageManager,
            IDocumentFilter documentFilter,
            IDownloadQueue downloadQueue,
            IConnection connection,
            INetworkSettings networkSettings)
            : base(storageManager, documentFilter, downloadQueue, connection, networkSettings)
        {
            _playOrResumePlayAction = playOrResumePlayAction;
            _documentsPOFactory = documentsPOFactory;
            _playOrResumePlayAction.AttachDataContext(this);
            
            AddToPlaylistCommand = new ExceptionHandlingCommand(async () => await AddToTrackCollection(Album.Id, DocumentType.Album));
            ShareCommand = new ExceptionHandlingCommand(async () => await shareLink.Share(_album));

            var audiobookStyler = new AudiobookPodcastInfoProvider(TrackInfoProvider);
            TrackInfoProvider = new CustomTrackInfoProvider(TrackInfoProvider,
                (track, culture, defaultTrack) =>
                {
                    if (audiobookStyler.HasSpecificStyling(track))
                        return audiobookStyler.GetTrackInformation(track, culture, defaultTrack);

                    if (track.Subtype == TrackSubType.Speech || track.Subtype == TrackSubType.Exegesis)
                    {
                        return new TrackInformation
                        {
                            Label = track.Artist,
                            Subtitle = defaultTrack.Subtitle,
                            Meta = string.IsNullOrWhiteSpace(track.Title) || track.Title == track.Artist ? ReadableDuration(track.Duration) : track.Title
                        };
                    }

                    return new TrackInformation
                    {
                        Label = track.Title,
                        Subtitle = defaultTrack.Subtitle,
                        Meta = string.IsNullOrWhiteSpace(track.Artist) || track.Artist == track.Title ? ReadableDuration(track.Duration) : track.Artist
                    };
                });
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            Documents.CollectionChanged += UpdateView;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            Documents.CollectionChanged -= UpdateView;
        }

        protected override Task DownloadAction()
        {
            return Task.CompletedTask;
        }

        protected override Task DeleteAction()
        {
            return Task.CompletedTask;
        }

        protected override Task<long> CalculateApproximateDownloadSize()
        {
            long sum = Documents
                .OfType<TrackPO>()
                .Sum(x => x.Track.Media.Sum(t => t.Files.Sum(s => s.Size)));
            return Task.FromResult(sum);
        }

        public override async Task Load()
        {
            await base.Load();
            RefreshButtons();
        }

        public string ReadableDuration(long durationInMilliseconds)
        {
            var span = TimeSpan.FromMilliseconds(durationInMilliseconds);
            var seconds = span.Seconds;
            var minutes = Math.Floor(span.TotalMinutes);
            return $"{minutes} minutes {seconds} seconds";
        }

        /// <summary>
        /// In case we already have a title we can show the title immediately without waiting for <see cref="LoadItems"/>.
        /// </summary>
        public void Prepare(Album album)
        {
            _id = album.Id;
            Album = album;
        }

        public void Prepare(int id)
        {
            _id = id;
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            Album = await Client.Albums.GetById(_id);
            return _documentsPOFactory.Create(
                Album?.Children,
                DocumentSelectedCommand,
                OptionCommand,
                TrackInfoProvider);
        }

        private void UpdateView(object sender, NotifyCollectionChangedEventArgs e) => RefreshButtons();

        private void RefreshButtons()
        {
            RaisePropertyChanged(() => ShowPlayButton);
            RaisePropertyChanged(() => ShowDownloadButtons);
        }

        public override string Title => Album?.Title;
        public override string Description => Album?.Description;
        public override bool ShowImage => Album?.Cover != null;
        public override string Image => Album?.Cover;
        public override bool ShowPlayButton => Documents.OfType<TrackPO>().Any();
        public override bool ShowDownloadButtons => Documents.OfType<TrackPO>().Any();
        public override string PlayButtonText => GetButtonText();

        private string GetButtonText()
        {
            if (Album != null && Album.LatestTrackId.HasValue)
                return TextSource[Translations.TrackCollectionViewModel_Resume];
            
            return TextSource[Translations.DocumentsViewModel_Play];
        }

        public override string TrackCountString => Documents.OfType<AlbumPO>().Any() ? TextSource.GetText(Translations.AlbumViewModel_PluralAlbums, Documents.OfType<AlbumPO>().Count()) : base.TrackCountString;
    }
}