using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
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
using BMM.Core.Translation;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels.Base
{
    public abstract class DocumentsViewModel : BaseViewModel
    {
        private IBlobCache _blobCache;
        private bool _isRefreshing;
        private bool _isInitialized;
        private IBmmObservableCollection<Document> _documents;
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

        public IMvxCommand PlayCommand { get; private set; }

        public IBmmObservableCollection<Document> Documents
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

        public virtual string TrackCountString => TextSource.GetText(Translations.DocumentsViewModel_PluralTracks, Documents.Count);

        public DocumentsViewModel(IDocumentFilter documentFilter = null)
        {
            Filter = documentFilter ?? new NullFilter();
            Documents = new BmmObservableCollection<Document>();

            CurrentTrack = Mvx.IoCProvider.Resolve<IMediaPlayer>().CurrentTrack;
            _currentTrackChangedToken = Messenger.Subscribe<CurrentTrackChangedMessage>(message =>
            {
                CurrentTrack = message.CurrentTrack;
            });

            ConnectionStatus = Mvx.IoCProvider.Resolve<IConnection>().GetStatus();
            _connectionStatusChangedToken = Messenger.Subscribe<ConnectionStatusChangedMessage>(message =>
            {
                ConnectionStatus = message.ConnectionStatus;
                RefreshDocumentsList();
            });

            PlayCommand = new ExceptionHandlingCommand(async () =>
            {
                var mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();
                var tracks = Documents.OfType<IMediaTrack>().ToList();

                if (tracks.Any())
                {
                    await mediaPlayer.Play(tracks, tracks.First(), PlaybackOriginString);
                }
            });

            ShufflePlayCommand = new ExceptionHandlingCommand(async () =>
            {
                var mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();
                var tracks = Documents.OfType<IMediaTrack>().ToList();

                if (tracks.Any())
                {
                    await mediaPlayer.ShuffleList(tracks, PlaybackOriginString);
                }
            });

            _trackMarkedAsListenedToken = Messenger.Subscribe<TrackMarkedAsListenedMessage>(HandleTrackMarkedAsListenedMessage);

            _fileDownloadStartedSubscriptionToken = Messenger.Subscribe<FileDownloadStartedMessage>(HandleFileDownloadStartedMessage);

            _fileDownloadCompletedSubscriptionToken = Messenger.Subscribe<FileDownloadCompletedMessage>(HandleFileDownloadCompletedMessage);

            _fileDownloadCanceledSubscriptionToken = Messenger.Subscribe<FileDownloadCanceledMessage>(HandleFileDownloadCanceledMessage);

            _downloadQueueChangedSubscriptionToken = Messenger.Subscribe<DownloadQueueChangedMessage>(HandleDownloadQueueChangedMessage);

            _downloadQueueFinishedSubscriptionToken = Messenger.Subscribe<QueueFinishedMessage>(HandleDownloadQueueFinishedMessage);
            _contentLanguageChangedToken = Messenger.Subscribe<ContentLanguagesChangedMessage>(HandleContentLanguageChanged);
        }

        [MvxInject]
        public IPostprocessDocumentsAction PostprocessDocumentsAction { get; set; }

        [MvxInject]
        public IMvxMainThreadAsyncDispatcher MvxMainThreadAsyncDispatcher { get; set; }

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

            _cacheToken = Messenger.Subscribe<CacheUpdatedMessage>(async message =>
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
                Messenger.Unsubscribe<CacheUpdatedMessage>(_cacheToken);
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
            ExceptionHandler.FireAndForgetWithoutUserMessages(() => LoadData(CachePolicy.IgnoreCache));
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
                await LoadData(CachePolicy.ForceGetAndUpdateCache);
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
            documents = await PostprocessDocumentsAction.ExecuteGuarded(documents);
            await ReplaceItems(documents);
        }

        protected async Task ReplaceItems(IEnumerable<Document> documents)
        {
            if (documents == null)
                return;

            await MvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                Documents.ReplaceWith(documents);
                RaisePropertyChanged(() => TrackCountString);
            });
        }

        protected override async Task DocumentAction(Document item, IList<Track> list)
        {
            await base.DocumentAction(item, FilteredDocuments(list).ToList());
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