using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.TrackOptions.Interfaces;
using BMM.Core.GuardedActions.TrackOptions.Parameters;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Dialogs;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Implementations.Tracks.Interfaces;
using BMM.Core.Models.POs.Albums;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Playlists;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Base
{
    /// <summary>
    /// Base class for all other ViewModels
    /// </summary>
    public class BaseViewModel : MvxViewModel, IBaseViewModel
    {
        private IMvxAsyncCommand _closeCommand;

        protected IExceptionHandler ExceptionHandler => Mvx.IoCProvider.Resolve<IExceptionHandler>();

        protected readonly IMvxMessenger Messenger;
        protected readonly IMvxNavigationService NavigationService;

        protected IBMMClient Client => Mvx.IoCProvider.Resolve<IBMMClient>();

        public BaseViewModel()
        {
            Messenger = Mvx.IoCProvider.Resolve<IMvxMessenger>();
            NavigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
        }

        [MvxInject]
        public INotificationCenter NotificationCenter { get; set; }

        [MvxInject]
        public IBMMLanguageBinder TextSource { get; set; }
        
        [MvxInject]
        public ITrackOptionsService TrackOptionsService { get; set; }        
        
        [MvxInject]
        public IPrepareTrackOptionsAction PrepareTrackOptionsAction { get; set; }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public virtual string PlaybackOriginString => string.Join("|", new List<string> { GetType().Name }.Concat(PlaybackOrigin()));

        public virtual IEnumerable<string> PlaybackOrigin()
        {
            return new List<string>();
        }

        private IMvxAsyncCommand<Document> _optionsCommand;

        public IMvxAsyncCommand<Document> OptionCommand
        {
            get
            {
                _optionsCommand ??= new ExceptionHandlingCommand<Document>(OptionsAction);
                return _optionsCommand;
            }
        }

        public IMvxAsyncCommand CloseCommand
        {
            get
            {
                _closeCommand ??= new MvxAsyncCommand(Close);
                return _closeCommand;
            }
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            AttachEvents();
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            DetachEvents();
        }

        public override void ViewCreated()
        {
            base.ViewCreated();
            NotificationCenter.AppLanguageChanged += NotificationCenterOnAppLanguageChanged;
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            NotificationCenter.AppLanguageChanged -= NotificationCenterOnAppLanguageChanged;
        }

        protected virtual void AttachEvents()
        {
        }

        protected virtual void DetachEvents()
        {
        }

        private void NotificationCenterOnAppLanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => TextSource);
        }

        private async Task Close(CancellationToken cancellationToken)
        {
            await NavigationService.Close(this, cancellationToken);
        }

        // ToDo: this method does not belong here. Create it's own class instead
        protected virtual async Task OptionsAction(Document item)
        {
            Mvx.IoCProvider.Resolve<IAnalytics>()
                .LogEvent(Event.OptionsMenuHasBeenOpened,
                    new Dictionary<string, object>
                    {
                        {"documentType", item.DocumentType}, {"id", item.Id}
                    });

            var bmmUserDialogs = Mvx.IoCProvider.Resolve<IBMMUserDialogs>();
            var isInOnlineMode = Mvx.IoCProvider.Resolve<IConnection>().GetStatus() == ConnectionStatus.Online;

            switch (item.DocumentType)
            {
                case DocumentType.Track:
                    await OpenTrackOptions((Track)item);
                    break;
                
                case DocumentType.Tile:
                    await OpenTrackOptions(((ContinueListeningTile)item).Track);
                    break;

                case DocumentType.Album:
                    var album = (Album)item;
                    bmmUserDialogs.ActionSheet(new ActionSheetConfig()
                        .SetTitle(album.Title)
                        .AddHandled(TextSource[Translations.UserDialogs_Album_AddToPlaylist], async () => await AddAlbumToTrackCollection(album.Id), ImageResourceNames.IconFavorites)
                        .AddHandled(TextSource[Translations.UserDialogs_Album_Share], async () => await Mvx.IoCProvider.Resolve<IShareLink>().Share(album), ImageResourceNames.IconShare)
                        .SetCancel(TextSource[Translations.UserDialogs_Cancel]));
                    break;

                case DocumentType.Contributor:
                    var contributor = (Contributor)item;
                    bmmUserDialogs.ActionSheet(new ActionSheetConfig()
                        .SetTitle(contributor.Name)
                        .AddHandled(TextSource[Translations.UserDialogs_Contributor_Share], async () => await Mvx.IoCProvider.Resolve<IShareLink>().Share(contributor), ImageResourceNames.IconShare)
                        .SetCancel(TextSource[Translations.UserDialogs_Cancel]));
                    break;

                case DocumentType.TrackCollection:
                    if (isInOnlineMode)
                    {
                        var trackCollection = (TrackCollection)item;

                        if (trackCollection.CanEdit)
                            ShowActionSheetIfPrivateTrackCollection(bmmUserDialogs, trackCollection, ImageResourceNames.IconShare, ImageResourceNames.IconRemove);
                        else
                            ShowActionSheetIfSharedTrackCollection(bmmUserDialogs, trackCollection, ImageResourceNames.IconRemove);
                    }
                    else
                    {
                        bmmUserDialogs.ActionSheet(new ActionSheetConfig()
                            .SetTitle(((TrackCollection)item).Name)
                            .SetCancel(TextSource[Translations.UserDialogs_Cancel])
                    );
                    }
                    break;

                case DocumentType.Podcast:
                    var podcastId = item.Id;
                    await NavigationService.Navigate<AutomaticDownloadViewModel, int>(podcastId);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task OpenTrackOptions(Track item)
        {
            var optionsList = await PrepareTrackOptionsAction.ExecuteGuarded(new PrepareTrackOptionsParameters(this, item));
            await TrackOptionsService.OpenOptions(optionsList);
        }

        private void ShowActionSheetIfSharedTrackCollection(
            IBMMUserDialogs userDialogs,
            TrackCollection trackCollection,
            string imageDelete)
        {
            userDialogs.ActionSheet(new ActionSheetConfig()
                .SetTitle(trackCollection.Name)
                .AddHandled(
                    TextSource[Translations.TrackCollectionViewModel_RemovePlaylist],
                    async () => await RemoveSharedPlaylist(trackCollection.Id),
                    imageDelete)
                .SetCancel(TextSource[Translations.UserDialogs_Cancel]));
        }

        private void ShowActionSheetIfPrivateTrackCollection(
            IBMMUserDialogs userDialogs,
            TrackCollection trackCollection,
            string imageShare,
            string imageDelete)
        {
            userDialogs.ActionSheet(new ActionSheetConfig()
                .SetTitle(trackCollection.Name)
                .AddHandled(TextSource[Translations.TrackCollectionViewModel_SharePlaylist],
                    async () =>
                    {
                        await ShareTrackCollection(trackCollection.Id);
                    }, imageShare)
                .AddHandled(TextSource[Translations.TrackCollectionViewModel_DeletePlaylist],
                    async () => await DeleteTrackCollection(trackCollection), imageDelete)
                .AddHandled(TextSource[Translations.TrackCollectionViewModel_EditPlaylist],
                    async () =>
                    {
                        await NavigationService.Navigate<EditTrackCollectionViewModel, ITrackCollectionParameter>(
                            new TrackCollectionParameter(trackCollection.Id));
                    },
                    ImageResourceNames.IconEdit)
                .SetCancel(TextSource[Translations.UserDialogs_Cancel])
            );
        }

        protected async Task ShareTrackCollection(int trackCollectionId)
        {
            await NavigationService.Navigate<ShareTrackCollectionViewModel, ITrackCollectionParameter>(
                new TrackCollectionParameter(trackCollectionId));
        }

        protected async Task RemoveSharedPlaylist(int trackCollectionId)
        {
            await Client.TrackCollection.Unfollow(trackCollectionId);
            await CloseCommand.ExecuteAsync();
        }

        private IMvxCommand<IDocumentPO> _documentSelectedCommand;

        public IMvxCommand<IDocumentPO> DocumentSelectedCommand => _documentSelectedCommand ??= new ExceptionHandlingCommand<IDocumentPO>(DocumentAction);

        protected virtual async Task DocumentAction(IDocumentPO item)
        {
            if (item != null)
                await DocumentAction(item, null);
        }

        /// <summary>
        /// Centralized action for what to do if you click on a document.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="list">List of tracks.</param>
        protected virtual async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            switch (item.DocumentType)
            {
                case DocumentType.Track:
                    var track = ((TrackPO)item).Track;
                    var mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();

                    if (list == null)
                    {
                        list = new List<Track> { track };
                    }
                    else if (list.All(t => t.Id != track.Id))
                    {
                        // I don't know how this is supposed to happen but the error logs show that it does
                        list.Add(track);
                    }

                    await mediaPlayer.Play(list.OfType<IMediaTrack>().ToList(), track, PlaybackOriginString);
                    break;

                case DocumentType.Album:
                    await NavigationService.Navigate<AlbumViewModel, Album>(((AlbumPO)item).Album);
                    break;

                case DocumentType.Contributor:
                    await NavigationService.Navigate<ContributorViewModel, int>(item.Id);
                    break;

                case DocumentType.TrackCollection:
                    var trackCollection = ((TrackCollectionPO)item).TrackCollection;
                    await NavigationService.Navigate<TrackCollectionViewModel, ITrackCollectionParameter>(new TrackCollectionParameter(trackCollection.Id, trackCollection.Name));
                    break;

                case DocumentType.Podcast:
                    var podcast = ((PodcastPO)item).Podcast;
                    await NavigationService.Navigate<PodcastViewModel, Podcast>(new Podcast {Id = podcast.Id, Title = podcast.Title});
                    break;

                case DocumentType.Playlist:
                    var playlist = ((PlaylistPO)item).Playlist;
                    await NavigationService.Navigate<CuratedPlaylistViewModel, Playlist>(new Playlist {Id = playlist.Id, Title = playlist.Title});
                    break;
            }
        }

        protected virtual async Task<bool> CreateTrackCollection()
        {
            var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().PromptAsync(TextSource[Translations.TrackCollectionViewModel_CreatePrompt]);
            if (result.Ok)
            {
                if (!string.IsNullOrEmpty(result.Text))
                {
                    TrackCollection collection = new TrackCollection
                    {
                        Name = result.Text
                    };

                    try
                    {
                        await Client.TrackCollection.Create(collection);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }

                    await Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(TextSource[Translations.TrackCollectionViewModel_CreateFailure]);
                }
                else
                {
                    await Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(TextSource[Translations.TrackCollectionViewModel_CreateEmptyFailure]);
                }
            }

            return false;
        }

        protected async Task UpdateTrackCollectionFiles(TrackCollection trackCollection)
        {
            try
            {
                var tcOfflineManager = Mvx.IoCProvider.Resolve<IOfflineTrackCollectionStorage>();

                if (tcOfflineManager.IsOfflineAvailable(trackCollection))
                {
                    var mediaDownloader = Mvx.IoCProvider.Resolve<IGlobalMediaDownloader>();

                    await Task.Run(mediaDownloader.SynchronizeOfflineTracks);
                }
            }
            catch (StorageOutOfSpaceException)
            {

            }
        }

        protected virtual async Task<bool> DeleteTrackCollection(TrackCollection item)
        {
            var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync(TextSource.GetText(Translations.TrackCollectionViewModel_DeleteConfirm, item.Name));
            if (!result)
            {
                return false;
            }

            try
            {
                if (await Client.TrackCollection.Delete(item.Id))
                {
                    var collectionManager = Mvx.IoCProvider.Resolve<ITrackCollectionManager>();
                    if (collectionManager.IsOfflineAvailable(item))
                    {
                        await collectionManager.RemoveDownloadedTrackCollection(item);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }

            await Mvx.IoCProvider.Resolve<IUserDialogs>().AlertAsync(TextSource.GetText(Translations.TrackCollectionViewModel_DeleteFailure, item.Name));
            return false;
        }

        protected Task AddAlbumToTrackCollection(int albumId)
        {
            return NavigationService.Navigate<TrackCollectionsAddToViewModel, TrackCollectionsAddToViewModel.Parameter>(new TrackCollectionsAddToViewModel.Parameter
            {
                DocumentId = albumId,
                DocumentType = DocumentType.Album
            });
        }
    }
}