using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Messages;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Base
{
    public abstract class DocumentsViewModel : BaseViewModel
    {
        private IBlobCache _blobCache;
        private bool _isRefreshing;
        private bool _isInitialized;
        private MvxObservableCollection<Document> _documents;
        private ITrackModel _currentTrack;
        public readonly IDocumentFilter Filter;

        private readonly MvxSubscriptionToken _fileDownloadStartedSubscriptionToken;
        private readonly MvxSubscriptionToken _fileDownloadCompletedSubscriptionToken;
        private readonly MvxSubscriptionToken _fileDownloadCanceledSubscriptionToken;
        private readonly MvxSubscriptionToken _downloadQueueChangedSubscriptionToken;
        private readonly MvxSubscriptionToken _downloadQueueFinishedSubscriptionToken;

        private readonly MvxSubscriptionToken _trackMarkedAsListenedToken;
        private readonly MvxSubscriptionToken _contentLanguageChangedToken;
        private readonly MvxSubscriptionToken _currentTrackChangedToken;
        private readonly MvxSubscriptionToken _connectionStatusChangedToken;
        private MvxSubscriptionToken _cacheToken;

        public ITrackInfoProvider TrackInfoProvider = new DefaultTrackInfoProvider();

        public ITrackModel CurrentTrack
        {
            get => _currentTrack;
            private set => SetProperty(ref _currentTrack, value);
        }

        public ConnectionStatus ConnectionStatus { get; private set; }

        public IBlobCache BlobCache
        {
            get => _blobCache ?? Akavache.BlobCache.UserAccount;
            set => _blobCache = value;
        }

        public IMvxAsyncCommand ReloadCommand => new ExceptionHandlingCommand(async () => await Refresh());

        public virtual bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        public IMvxCommand ShufflePlayCommand { get; private set; }

        public MvxObservableCollection<Document> Documents
        {
            get => _documents;
            private set => SetProperty(ref _documents, value);
        }

        protected override async Task DocumentAction(Document item)
        {
            if (item != null)
                await DocumentAction(item, Documents.OfType<Track>().ToList());
        }

        public IList<T> FilteredDocuments<T>(IList<T> list) where T : Document
        {
            return list.Where(Filter.WherePredicate).ToList();
        }

        public IMvxLanguageBinder DocumentsTextSource => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(DocumentsViewModel));

        public virtual string TrackCountString => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(DocumentsViewModel))
            .GetText("PluralTracks", Documents.Count);

        public DocumentsViewModel(IDocumentFilter documentFilter = null, IMvxLanguageBinder textSource = null)
            : base(textSource)
        {
            Filter = documentFilter ?? new NullFilter();
            Documents = new MvxObservableCollection<Document>();

            CurrentTrack = Mvx.IoCProvider.Resolve<IMediaPlayer>().CurrentTrack;
            _currentTrackChangedToken = _messenger.Subscribe<CurrentTrackChangedMessage>(message =>
            {
                CurrentTrack = message.CurrentTrack;
            });

            ConnectionStatus = Mvx.IoCProvider.Resolve<IConnection>().GetStatus();
            _connectionStatusChangedToken = _messenger.Subscribe<ConnectionStatusChangedMessage>(message =>
            {
                ConnectionStatus = message.ConnectionStatus;
                RefreshDocumentsList();
            });

            ShufflePlayCommand = new ExceptionHandlingCommand(async () =>
            {
                var mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();
                var tracks = Documents.OfType<IMediaTrack>().ToList();

                if (tracks.Any())
                {
                    await mediaPlayer.ShuffleList(tracks, GetType().Name);
                }
            });

            _trackMarkedAsListenedToken = _messenger.Subscribe<TrackMarkedAsListenedMessage>(HandleTrackMarkedAsListenedMessage);

            _fileDownloadStartedSubscriptionToken = _messenger.Subscribe<FileDownloadStartedMessage>(HandleFileDownloadStartedMessage);

            _fileDownloadCompletedSubscriptionToken = _messenger.Subscribe<FileDownloadCompletedMessage>(HandleFileDownloadCompletedMessage);

            _fileDownloadCanceledSubscriptionToken = _messenger.Subscribe<FileDownloadCanceledMessage>(HandleFileDownloadCanceledMessage);

            _downloadQueueChangedSubscriptionToken = _messenger.Subscribe<DownloadQueueChangedMessage>(HandleDownloadQueueChangedMessage);

            _downloadQueueFinishedSubscriptionToken = _messenger.Subscribe<QueueFinishedMessage>(HandleDownloadQueueFinishedMessage);
            _contentLanguageChangedToken = _messenger.Subscribe<ContentLanguagesChangedMessage>(HandleContentLanguageChanged);
        }

        private void HandleContentLanguageChanged(ContentLanguagesChangedMessage obj)
        {
            ExceptionHandler.FireAndForgetWithoutUserMessages(Refresh);
        }

        protected void HandleTrackMarkedAsListenedMessage(TrackMarkedAsListenedMessage message)
        {
            foreach (var document in Documents.OfType<Track>())
            {
                if (document.Id == message.TrackId)
                {
                    document.IsListened = true;
                    RaisePropertyChanged(() => Documents);
                }
            }
        }

        protected virtual void HandleDownloadQueueChangedMessage(DownloadQueueChangedMessage obj)
        {
            RefreshDocumentsList();
        }

        protected virtual void HandleFileDownloadStartedMessage(FileDownloadStartedMessage message)
        {
            RefreshDocumentsList();
        }

        protected virtual void HandleFileDownloadCompletedMessage(FileDownloadCompletedMessage message)
        {
            RefreshDocumentsList();
        }

        protected virtual void HandleFileDownloadCanceledMessage(FileDownloadCanceledMessage message)
        {
            RefreshDocumentsList();
        }

        protected virtual void HandleDownloadQueueFinishedMessage(QueueFinishedMessage message)
        {
            RefreshDocumentsList();
        }

        protected virtual Task Initialization()
        {
            return Load();
        }

        public virtual CacheKeys? CacheKey => null;

        protected void SubscribeToChanges()
        {
            if (!CacheKey.HasValue)
            {
                return;
            }

            _cacheToken = _messenger.Subscribe<CacheUpdatedMessage>(async message =>
            {
                if (message.Type == CacheKey)
                    await RefreshInBackgroundAfterCacheUpdate();
            });

            // ToDo: By default subscribe to nothing but make it possible to subscribe to Podcast with ID 1 etc.
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);

            if (_cacheToken != null)
            {
                _messenger.Unsubscribe<CacheUpdatedMessage>(_cacheToken);
            }
        }

        public sealed override async Task Initialize()
        {
            await base.Initialize();
            try
            {
                await Initialization();
                SubscribeToChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            finally
            {
                IsInitialized = true;
            }

            // ToDo: show something useful in case nothing can be loaded
        }

        public abstract Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated);

        public async Task TryRefresh()
        {
            try
            {
                await Refresh();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        /// <summary>
        /// Run a task in Background without actually waiting for it. This is needed for <see cref="Initialization"/> to do work without waiting for the result.
        /// </summary>
        protected Task BackgroundInitialization(Func<Task> action)
        {
            ExceptionHandler.FireAndForgetWithoutUserMessages(action);

            // we could just change this method to return void. But this way it's more interchangeable with other tasks that don't run in background.
            return Task.CompletedTask;
        }

        public virtual void RefreshInBackground()
        {
            ExceptionHandler.FireAndForgetWithoutUserMessages(() => LoadData(CachePolicy.UseCacheAndWaitForUpdates));
        }

        public virtual async Task RefreshInBackgroundAfterCacheUpdate()
        {
            try
            {
                await LoadData(CachePolicy.UseCache);
            }
            catch (Exception ex)
            {
                var handler = Mvx.IoCProvider.Resolve<IExceptionHandler>();
                handler.HandleExceptionWithoutUserMessages(ex);
            }
        }

        public virtual async Task Refresh()
        {
            if (IsRefreshing)
            {
                return;
            }

            IsRefreshing = true;
            try
            {
                await LoadData(CachePolicy.BypassCache);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public virtual async Task Load()
        {
            if (IsLoading)
            {
                return;
            }

            IsLoading = true;
            try
            {
                await LoadData();
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected async Task LoadData(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var documents = await LoadItems(policy);
            documents = ExcludeVideos(documents);
            documents = await EnrichDocumentsWithAdditionalData(documents);
            await ReplaceItems(documents);
        }

        protected async Task<IEnumerable<Document>> EnrichDocumentsWithAdditionalData(IEnumerable<Document> documents)
        {
            if (documents == null)
                return null;

            var docList = documents.ToList();
            var listenedTracksStorage = Mvx.IoCProvider.Resolve<IListenedTracksStorage>();
            foreach (var doc in docList)
            {
                if (doc is Track track)
                    track.IsListened = await listenedTracksStorage.TrackIsListened(track);
            }

            return docList;
        }

        protected Task ReplaceItems(IEnumerable<Document> documents)
        {
            return Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                .ExecuteOnMainThreadAsync(() =>
                {
                    if (documents != null)
                    {
                        Documents.ReplaceWith(documents);
                        RaisePropertyChanged(() => TrackCountString);
                    }
                });
        }

        protected override async Task DocumentAction(Document item, IList<Track> list)
        {
            await base.DocumentAction(item, FilteredDocuments(list).ToList());
        }

        public IEnumerable<Document> ExcludeVideos(IEnumerable<Document> documents)
        {
            if (documents == null)
            {
                return null;
            }

            IEnumerable<Track> videoDocuments = documents.OfType<Track>().Where(t => t.Subtype == TrackSubType.Video);
            return documents.Where(d => videoDocuments.All(v => v.Id != d.Id)); // Filter out documents of video type
        }

        ///<summary>
        /// Unfortunately it's not able to only refresh a single item.
        /// Therefore we raise property changed on the whole list which makes all items being re-rendered.
        /// To fix it we would need Document to implement INotifyPropertyChanged
        /// </summary>
        private void RefreshDocumentsList()
        {
            RaisePropertyChanged(() => Documents);
            RaisePropertyChanged(() => TrackCountString);
        }
    }
}