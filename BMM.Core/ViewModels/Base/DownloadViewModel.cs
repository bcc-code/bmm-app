using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Framework;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages;
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
            }
        }

        private int ToBeDownloadedCount => DownloadQueue.InitialDownloadCount;

        private int DownloadedFilesCount => DownloadQueue.InitialDownloadCount - DownloadQueue.RemainingDownloadsCount;

        private IEnumerable<IDownloadFile> _downloadingFiles;

        protected IEnumerable<IDownloadFile> DownloadingFiles
        {
            get => _downloadingFiles;
            private set => SetProperty(ref _downloadingFiles, value);
        }

        public bool IsDownloading => IsOfflineAvailable && DownloadedFilesCount < ToBeDownloadedCount && ToBeDownloadedCount > 0;

        public bool ShowDownloadButtons => true;

        public bool IsDownloaded => IsOfflineAvailable && !IsDownloading;

        public abstract string Title { get; }

        public virtual string Description => null;

        public virtual bool ShowPlaylistIcon => false;

        public virtual bool ShowFollowSharedPlaylistButton => false;

        public abstract string Image { get; }

        public bool UseCircularImage => false;

        public bool ShowFollowButtons => false;

        public bool ShowShuffleButton => true;
        public bool ShowPlayButton => false;

        public bool ShowTrackCount => true;

        public string DownloadingText => !IsDownloading
            ? ""
            : TextSource.GetText("AvailableOfflineDownloading",
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

            _messenger.Subscribe<DownloadCanceledMessage>(async message =>
                {
                    if (IsDownloading && IsOfflineAvailable)
                    {
                        await DeleteAction();

                        IsOfflineAvailable = !IsOfflineAvailable;
                        await RaisePropertyChanged(() => Documents);
                    }
                },
                MvxReference.Strong);
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

        public override async Task RefreshInBackgroundAfterCacheUpdate()
        {
            await base.RefreshInBackgroundAfterCacheUpdate();

            // Since the max age for a TrackCollection is 0 this will always be executed when opening a TrackCollection
            if (IsOfflineAvailable)
                await ResumeDownloading();
        }

        protected abstract Task DownloadAction();

        protected abstract Task DeleteAction();

        // todo find out what the hack this is mb/gb?????
        protected abstract Task<long> CalculateApproximateDownloadSize();

        protected async Task ToggleOffline()
        {
            // TODO: Find a better way to show the user that downloading can't be triggered if the playlist hasn't been fully loaded.
            if (IsLoading)
                return;

            var newIsOfflineAvailable = !IsOfflineAvailable;

            if (newIsOfflineAvailable)
            {
                var mobileNetworkDownloadAllowed = await _networkSettings.GetMobileNetworkDownloadAllowed();

                var isUsingNetworkWithoutExtraCosts = Connection.IsUsingNetworkWithoutExtraCosts();

                if (!mobileNetworkDownloadAllowed && !isUsingNetworkWithoutExtraCosts)
                {
                    await Mvx.IoCProvider.Resolve<IToastDisplayer>().WarnAsync(TextSource.GetText("MobileDownloadDisabled"));
                    return;
                }

                if (_storageManager.SelectedStorage.FreeSpace <= await CalculateApproximateDownloadSize())
                {
                    await Mvx.IoCProvider.Resolve<IToastDisplayer>().WarnAsync(TextSource.GetText("NotEnoughtSpaceToDownload"));
                    return;
                }

                IsOfflineAvailable = newIsOfflineAvailable;

                await DownloadAction();
                await RaisePropertyChanged(() => IsDownloaded);
            }
            else
            {
                // todo fix this get the name fo the entity to delete
                var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync(TextSource.GetText("RemoveOfflineConfirm"));
                if (!result)
                {
                    return;
                }

                IsOfflineAvailable = newIsOfflineAvailable;

                await DeleteAction();

                await RaisePropertyChanged(() => Documents);
                await RaisePropertyChanged(() => IsDownloaded);
            }
        }

        protected Task ResumeDownloading()
        {
            // todo do not really get why this is an own method
            return DownloadAction();
        }

        private void RaiseDownloadProgressChanged()
        {
            RaisePropertyChanged(() => IsDownloading);
            RaisePropertyChanged(() => IsDownloaded);
            RaisePropertyChanged(() => DownloadingText);
            RaisePropertyChanged(() => DownloadStatus);
        }
    }
}