using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels.Base
{
    public abstract class DownloadViewModel : DocumentsViewModel, ITrackListViewModel
    {
        private bool _isOfflineAvailable;

        public bool IsOfflineAvailable
        {
            get => _isOfflineAvailable;
            protected set
            {
                SetProperty(ref _isOfflineAvailable, value);
                RaisePropertyChanged(() => IsDownloaded);
                RaisePropertyChanged(() => IsDownloading);
                RaisePropertyChanged(() => ShowDownloadButton);
            }
        }

        /// <summary>
        /// Whether every track we expect offline is actually present on disk.
        /// </summary>
        private bool AreAllTracksDownloaded
        {
            get => _areAllTracksDownloaded;
            set
            {
                if (_areAllTracksDownloaded == value)
                    return;

                _areAllTracksDownloaded = value;
                RaisePropertyChanged(() => IsDownloaded);
                RaisePropertyChanged(() => ShowDownloadButton);
            }
        }

        /// <summary>
        /// The tracks whose files are expected on disk once this item is downloaded. Empty by default, so
        /// view models that have not opted in keep behaving as before.
        /// </summary>
        protected virtual IEnumerable<IDownloadable> DownloadableTracks => Enumerable.Empty<IDownloadable>();

        /// <summary>
        /// Re-reads which files are on disk. Cached rather than computed per property read, because the
        /// download messages raise these properties dozens of times during a single queue run.
        /// </summary>
        protected void RefreshDownloadedFilesState()
        {
            var tracks = DownloadableTracks?.ToList();

            // An item we know nothing about yet must not be reported as incomplete, or opening a
            // collection would briefly claim its downloads are missing.
            AreAllTracksDownloaded = tracks == null
                                     || tracks.Count == 0
                                     || tracks.All(track => _storageManager.SelectedStorage.IsDownloaded(track));
        }

        private int ToBeDownloadedCount => DownloadQueue.InitialDownloadCount;

        private int DownloadedFilesCount => DownloadQueue.InitialDownloadCount - DownloadQueue.RemainingDownloadsCount;

        private IEnumerable<IDownloadFile> _downloadingFiles;

        protected IEnumerable<IDownloadFile> DownloadingFiles
        {
            get => _downloadingFiles;
            private set => SetProperty(ref _downloadingFiles, value);
        }

        /// <summary>
        /// Requires the queue to actually be running. Items left behind by a queue that stopped early
        /// would otherwise keep the progress indicator on screen forever, with nothing downloading.
        /// </summary>
        public bool IsDownloading => IsOfflineAvailable
                                     && DownloadQueue.IsRunning
                                     && DownloadedFilesCount < ToBeDownloadedCount
                                     && ToBeDownloadedCount > 0;

        public virtual bool ShowDownloadButtons => true;

        /// <summary>
        /// "The user asked for this offline" and "the files are here" are different claims. This used to
        /// be <c>IsOfflineAvailable &amp;&amp; !IsDownloading</c>, which meant an empty download queue was
        /// indistinguishable from a finished one: a playlist whose downloads all failed rendered with a
        /// checkmark and no files on disk.
        /// </summary>
        public bool IsDownloaded => IsOfflineAvailable && !IsDownloading && AreAllTracksDownloaded;

        /// <summary>
        /// Offer the download action whenever the files are not all there, so an incomplete download can
        /// be retried instead of hiding behind a checkmark.
        /// </summary>
        public bool ShowDownloadButton => ShowDownloadButtons && !IsDownloading && !IsDownloaded;

        public abstract string Title { get; }

        public virtual string Description => null;

        public virtual bool ShowPlaylistIcon => false;

        public virtual string PlayButtonText => TextSource[Translations.TrackCollectionViewModel_ShufflePlay];


        public abstract string Image { get; }

        public bool UseCircularImage => false;

        public bool ShowFollowButtons => false;

        public virtual bool ShowPlayButton => true;
        public bool ShowTrackCount => true;

        public string DurationLabel
        {
            get => _durationLabel;
            set => SetProperty(ref _durationLabel, value);
        }

        public bool IsCompletedPercentageVisible
        {
            get => _isCompletedPercentageVisible;
            set => SetProperty(ref _isCompletedPercentageVisible, value);
        }

        public string DownloadingText => !IsDownloading
            ? ""
            : TextSource.GetText(Translations.TrackCollectionViewModel_AvailableOfflineDownloading,
                (ToBeDownloadedCount - DownloadedFilesCount).ToString(),
                ToBeDownloadedCount.ToString());

        public float DownloadStatus
        {
            get
            {
                if (!IsDownloading && !Documents.Any()) return 0f;
                var progress = 1.0f / ToBeDownloadedCount * DownloadedFilesCount;
                return progress;
            }
        }

        public IMvxAsyncCommand ToggleOfflineCommand { get; private set; }

        private readonly IStorageManager _storageManager;

        protected readonly IDownloadQueue DownloadQueue;
        protected readonly IConnection Connection;
        private readonly INetworkSettings _networkSettings;
        private MvxSubscriptionToken _downloadCancelledMessageToken;
        private string _durationLabel;
        private bool _isCompletedPercentageVisible;
        private bool _areAllTracksDownloaded = true;

        public virtual bool ShowSharingInfo => false;
        public virtual bool ShowImage => true;

        public DownloadViewModel(
            IStorageManager storageManager,
            IDocumentFilter documentFilter,
            IDownloadQueue downloadQueue,
            IConnection connection,
            INetworkSettings networkSettings)
            : base(documentFilter)
        {
            _storageManager = storageManager;
            DownloadQueue = downloadQueue;
            Connection = connection;
            _networkSettings = networkSettings;

            ToggleOfflineCommand = new ExceptionHandlingCommand(async () => await ToggleOffline());
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            _downloadCancelledMessageToken = Messenger.Subscribe<DownloadCanceledMessage>(async message =>
            {
                if (IsDownloading && IsOfflineAvailable)
                {
                    await DeleteAction();

                    IsOfflineAvailable = !IsOfflineAvailable;
                    RefreshAllTracks();
                }
            });
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            Messenger.UnsubscribeSafe<DownloadCanceledMessage>(_downloadCancelledMessageToken);
        }

        protected override void HandleFileDownloadStartedMessage(FileDownloadStartedMessage message)
        {
            base.HandleFileDownloadStartedMessage(message);
            RaiseDownloadProgressChanged();
        }

        protected override void HandleFileDownloadCompletedMessage(FileDownloadCompletedMessage message)
        {
            base.HandleFileDownloadCompletedMessage(message);
            RaiseDownloadProgressChanged();
        }

        protected override void HandleFileDownloadCanceledMessage(FileDownloadCanceledMessage message)
        {
            base.HandleFileDownloadCanceledMessage(message);
            RaiseDownloadProgressChanged();
        }

        protected override void HandleDownloadQueueChangedMessage(DownloadQueueChangedMessage obj)
        {
            base.HandleDownloadQueueChangedMessage(obj);
            RaiseDownloadProgressChanged();
        }

        protected override void HandleDownloadQueueFinishedMessage(QueueFinishedMessage message)
        {
            base.HandleDownloadQueueFinishedMessage(message);
            RaiseDownloadProgressChanged();
        }

        public override async Task Load()
        {
            await base.Load();
            RefreshDownloadedFilesState();
        }

        public override async Task RefreshInBackgroundAfterCacheUpdate()
        {
            await base.RefreshInBackgroundAfterCacheUpdate();
            RefreshDownloadedFilesState();

            // Since the max age for a TrackCollection is 0 this will always be executed when opening a TrackCollection
            if (IsOfflineAvailable)
                await ResumeDownloading();
        }

        protected abstract Task DownloadAction();

        protected abstract Task DeleteAction();

        // todo find out what the hack this is mb/gb?????
        protected abstract Task<long> CalculateApproximateDownloadSize();

        protected virtual string PrepareDurationLabel(TimeSpan duration)
        {
            string minText = TextSource[Translations.AlbumViewModel_Minute];
            string secText = TextSource[Translations.AlbumViewModel_Second];
            
            if (duration.Hours == 0)
                return $"{duration.Minutes} {minText} {duration.Seconds} {secText}";
            
            return $"{duration.Hours} {GetHoursText(duration)}, {duration.Minutes} {minText}";
        }
        
        private string GetHoursText(TimeSpan time)
        {
            return time.Hours == 1
                ? TextSource.GetText(Translations.AlbumViewModel_Hour) 
                : TextSource.GetText(Translations.AlbumViewModel_Hours);
        }

        protected async Task ToggleOffline()
        {
            // TODO: Find a better way to show the user that downloading can't be triggered if the playlist hasn't been fully loaded.
            if (IsLoading)
                return;

            bool newIsOfflineAvailable = !IsOfflineAvailable;

            if (newIsOfflineAvailable)
            {
                bool mobileNetworkDownloadAllowed = await _networkSettings.GetMobileNetworkDownloadAllowed();
                bool isUsingNetworkWithoutExtraCosts = Connection.IsUsingNetworkWithoutExtraCosts();

                if (_storageManager.SelectedStorage.FreeSpace <= await CalculateApproximateDownloadSize())
                {
                    await Mvx.IoCProvider.Resolve<IToastDisplayer>().WarnAsync(TextSource[Translations.TrackCollectionViewModel_NotEnoughtSpaceToDownload]);
                    return;
                }
                
                if (!mobileNetworkDownloadAllowed && !isUsingNetworkWithoutExtraCosts)
                    await Mvx.IoCProvider.Resolve<IToastDisplayer>().WarnAsync(TextSource[Translations.Global_DownloadPlaylistOnceOnWifi]);
                
                IsOfflineAvailable = newIsOfflineAvailable;

                await DownloadAction();
                RefreshDownloadedFilesState();
                await RaisePropertyChanged(() => IsDownloaded);
                await RaisePropertyChanged(() => ShowDownloadButton);
            }
            else
            {
                // todo fix this get the name fo the entity to delete
                var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync(TextSource[Translations.TrackCollectionViewModel_RemoveOfflineConfirm]);
                if (!result)
                {
                    return;
                }

                IsOfflineAvailable = newIsOfflineAvailable;

                await DeleteAction();

                RefreshAllTracks();
                RefreshDownloadedFilesState();
                await RaisePropertyChanged(() => IsDownloaded);
                await RaisePropertyChanged(() => ShowDownloadButton);
            }
        }

        protected Task ResumeDownloading()
        {
            // todo do not really get why this is an own method
            return DownloadAction();
        }

        private void RaiseDownloadProgressChanged()
        {
            // Skipped while downloading: the checkmark is not on screen then, and re-checking every file
            // on each of the many progress messages would be wasted work.
            if (!IsDownloading)
                RefreshDownloadedFilesState();

            RaisePropertyChanged(() => IsDownloading);
            RaisePropertyChanged(() => IsDownloaded);
            RaisePropertyChanged(() => ShowDownloadButton);
            RaisePropertyChanged(() => DownloadingText);
            RaisePropertyChanged(() => DownloadStatus);
        }
    }
}