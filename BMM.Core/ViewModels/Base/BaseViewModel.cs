using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Acr.UserDialogs;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Localization;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using Xamarin.Essentials;

namespace BMM.Core.ViewModels.Base
{
    /// <summary>
    /// Base class for all other ViewModels
    /// </summary>
    public class BaseViewModel : MvxViewModel
    {
        private IMvxAsyncCommand _closeCommand;

        protected IExceptionHandler ExceptionHandler => Mvx.IoCProvider.Resolve<IExceptionHandler>();

        protected readonly IMvxMessenger _messenger;

        protected readonly IMvxNavigationService _navigationService;

        protected IBMMClient Client => Mvx.IoCProvider.Resolve<IBMMClient>();

        public BaseViewModel()
        {
            Mvx.IoCProvider.Resolve<INotificationCenter>().AppLanguageChanged += (sender, e) =>
            {
                RaisePropertyChanged(() => TextSource);
            };
            _messenger = Mvx.IoCProvider.Resolve<IMvxMessenger>();
            _navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
        }

        [MvxInject]
        public IBMMLanguageBinder TextSource { get; set; }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private IMvxAsyncCommand<Document> _optionsCommand;

        public IMvxAsyncCommand<Document> OptionCommand
        {
            get
            {
                _optionsCommand = _optionsCommand ?? new ExceptionHandlingCommand<Document>(OptionsAction);
                return _optionsCommand;
            }
        }

        private IMvxCommand<Track> _showTrackInfoCommand;

        public IMvxCommand<Track> ShowTrackInfoCommand
        {
            get
            {
                _showTrackInfoCommand = _showTrackInfoCommand ?? new ExceptionHandlingCommand<Track>(ShowTrackInfo);
                return _showTrackInfoCommand;
            }
        }

        public IMvxAsyncCommand CloseCommand
        {
            get
            {
                _closeCommand = _closeCommand ?? new MvxAsyncCommand(Close);
                return _closeCommand;
            }
        }

        private async Task Close(CancellationToken cancellationToken)
        {
            await _navigationService.Close(this, cancellationToken);
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

            var isInOnlineMode = Mvx.IoCProvider.Resolve<IConnection>().GetStatus() == ConnectionStatus.Online;
            var imageDelete = "icon_trash_static.png";
            var imageAddTo = "icon_nav_content_static.png";
            var imageShare = "icon_share.png";

            switch (item.DocumentType)
            {
                case DocumentType.Track:
                    var track = item as Track;
                    if (track == null)
                        throw new NotSupportedException("A document with type Track has to be castable to Track");

                    var actionSheet = new ActionSheetConfig();

                    if (isInOnlineMode && !track.IsLivePlayback)
                    {
                        if (this is TrackCollectionViewModel trackCollectionVm)
                        {
                            actionSheet.AddHandled(TextSource.GetText(Translations.UserDialogs_Track_DeleteFromPlaylist),
                                async () => await trackCollectionVm.DeleteTrackFromTrackCollection((Track)item),
                                imageDelete);
                        }

                        actionSheet.AddHandled(TextSource.GetText(Translations.UserDialogs_Track_AddToPlaylist),
                            async () =>
                            {
                                _messenger.Publish(new TogglePlayerMessage(this, false));
                                await _navigationService.Navigate<TrackCollectionsAddToViewModel, TrackCollectionsAddToViewModel.Parameter>(new TrackCollectionsAddToViewModel.Parameter
                                {
                                    DocumentId = item.Id,
                                    DocumentType = item.DocumentType
                                });
                            },
                            imageAddTo);
                    }

                    if (!(this is QueueViewModel) && !(this is PlayerViewModel))
                    {
                        const string imageQueue = "icon_play_static.png";
                        var mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();

                        // todo #17871 enable for android as well
                        if (DeviceInfo.Platform == DevicePlatform.iOS)
                        {
                            actionSheet.AddHandled(TextSource[Translations.UserDialogs_Track_QueueToPlayNext],
                                async () =>
                                {
                                    var success = await mediaPlayer.QueueToPlayNext(track, GetType().Name);
                                    if (success)
                                    {
                                        await Mvx.IoCProvider.Resolve<IToastDisplayer>().Success(TextSource.GetText(Translations.UserDialogs_Track_AddedToQueue, track.Title));

                                        Mvx.IoCProvider.Resolve<IAnalytics>()
                                            .LogEvent(Event.TrackHasBeenAddedToBePlayedNext,
                                                new Dictionary<string, object>
                                                {
                                                    {"track", track.Id}
                                                });
                                    }
                                },
                                imageQueue);
                        }
                        actionSheet.AddHandled(TextSource[Translations.UserDialogs_Track_AddToQueue],
                            async () =>
                            {
                                var success = await mediaPlayer.AddToEndOfQueue(track, GetType().Name);
                                if (success)
                                {
                                    await Mvx.IoCProvider.Resolve<IToastDisplayer>().Success(TextSource.GetText(Translations.UserDialogs_Track_AddedToQueue, track.Title));

                                    Mvx.IoCProvider.Resolve<IAnalytics>()
                                        .LogEvent(Event.TrackHasBeenAddedToEndOfQueue,
                                            new Dictionary<string, object>
                                            {
                                                {"track", track.Id}
                                            });
                                }
                            },
                            imageQueue);
                    }

                    if (!track.IsLivePlayback)
                    {
                        actionSheet.AddHandled(TextSource[Translations.UserDialogs_Track_Share], async () => await Mvx.IoCProvider.Resolve<IShareLink>().For(track), imageShare);
                    }

                    // Only show this option if we are not inside an album (what apparently is the one you would be guided to here)
                    if (!(this is AlbumViewModel) && isInOnlineMode && track.ParentId != 0)
                    {
                        var imageAlbum = "icon_nav_album_static.png";
                        actionSheet.AddHandled(TextSource[Translations.UserDialogs_Track_GoToAlbum],
                            async () =>
                            {
                                _messenger.Publish(new TogglePlayerMessage(this, false));
                                await _navigationService.Navigate<AlbumViewModel, Album>(new Album {Id = track.ParentId, Title = track.Album});
                            },
                            imageAlbum);
                    }
                    if (isInOnlineMode)
                    {
                        // Add all contributors to a separate list ...
                        // TODO: Hide this option only if you are on that contributors page.
                        IList<TrackRelation> contributors = ((Track)item).Relations?.Where(r =>
                            r.Type == TrackRelationType.Composer ||
                            r.Type == TrackRelationType.Interpret ||
                            r.Type == TrackRelationType.Lyricist ||
                            r.Type == TrackRelationType.Arranger)
                            .ToList();

                        if (contributors != null && contributors.Count > 0)
                        {
                            var imageContributor = "icon_nav_contributor_static.png";
                            var trackHasOnlyOneContributor = contributors.Count == 1;

                            actionSheet.AddHandled(TextSource[Translations.UserDialogs_Track_GoToContributor], async () =>
                            {
                                var contributorConfig = new ActionSheetConfig();

                                foreach (TrackRelation relation in ((Track)item).Relations)
                                {
                                    if (relation.Type == TrackRelationType.Composer)
                                    {
                                        var rel = (TrackRelationComposer)relation;

                                        if (trackHasOnlyOneContributor)
                                            await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                        else
                                            contributorConfig.AddHandled(TextSource.GetText(Translations.UserDialogs_Track_GoToContributor_Composer, rel.Name),
                                                async () =>
                                                {
                                                    _messenger.Publish(new TogglePlayerMessage(this, false));
                                                    await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                                });

                                    }
                                    else if (relation.Type == TrackRelationType.Interpret)
                                    {
                                        var rel = (TrackRelationInterpreter)relation;

                                        if (trackHasOnlyOneContributor)
                                            await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                        else
                                            contributorConfig.AddHandled(TextSource.GetText(Translations.UserDialogs_Track_GoToContributor_Interpret, rel.Name),
                                                async () =>
                                                {
                                                    _messenger.Publish(new TogglePlayerMessage(this, false));
                                                    await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                                });
                                    }
                                    else if (relation.Type == TrackRelationType.Lyricist)
                                    {
                                        var rel = (TrackRelationLyricist)relation;

                                        if (trackHasOnlyOneContributor)
                                            await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                        else
                                            contributorConfig.AddHandled(TextSource.GetText(Translations.UserDialogs_Track_GoToContributor_Lyricist, rel.Name),
                                                async () =>
                                                {
                                                    _messenger.Publish(new TogglePlayerMessage(this, false));
                                                    await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                                });
                                    }
                                    else if (relation.Type == TrackRelationType.Arranger)
									{
										var rel = (TrackRelationArranger)relation;

                                        if (trackHasOnlyOneContributor)
                                            await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                        else
                                            contributorConfig.AddHandled(TextSource.GetText(Translations.UserDialogs_Track_GoToContributor_Arranger, rel.Name),
                                                async () =>
                                                {
                                                    _messenger.Publish(new TogglePlayerMessage(this, false));
                                                    await _navigationService.Navigate<ContributorViewModel, int>(rel.Id);
                                                });
                                    }
                                }

                                contributorConfig.SetCancel(TextSource[Translations.UserDialogs_Cancel]);

                                if (!trackHasOnlyOneContributor)
                                    Mvx.IoCProvider.Resolve<IUserDialogs>().ActionSheet(contributorConfig);

                            }, imageContributor);
                        }
                    }
                    var imageInfo = "icon_info_static.png";
                    actionSheet.AddHandled(TextSource[Translations.UserDialogs_Track_MoreInformation],
                        async () => { await ShowTrackInfo((Track)item); },
                        imageInfo);


                    actionSheet.SetCancel(TextSource[Translations.UserDialogs_Cancel]);

                    Mvx.IoCProvider.Resolve<IUserDialogs>().ActionSheet(actionSheet);
                    break;

                case DocumentType.Album:
                    var album = (Album)item;
                    Mvx.IoCProvider.Resolve<IUserDialogs>().ActionSheet(new ActionSheetConfig()
                        .SetTitle(album.Title)
                        .AddHandled(TextSource[Translations.UserDialogs_Album_AddToPlaylist], async () => await AddAlbumToTrackCollection(album.Id), imageAddTo)
                        .AddHandled(TextSource[Translations.UserDialogs_Album_Share], async () => await Mvx.IoCProvider.Resolve<IShareLink>().For(album), imageShare)
                        .SetCancel(TextSource[Translations.UserDialogs_Cancel])
                    );
                    break;

                case DocumentType.Contributor:
                    var contributor = (Contributor)item;
                    Mvx.IoCProvider.Resolve<IUserDialogs>().ActionSheet(new ActionSheetConfig()
                        .SetTitle(contributor.Name)
                        .AddHandled(TextSource[Translations.UserDialogs_Contributor_Share], async () => await Mvx.IoCProvider.Resolve<IShareLink>().For(contributor), imageShare)
                        .SetCancel(TextSource[Translations.UserDialogs_Cancel])
                    );
                    break;

                case DocumentType.TrackCollection:
                    if (isInOnlineMode)
                    {
                        var trackCollection = (TrackCollection)item;

                        if (trackCollection.CanEdit)
                            ShowActionSheetIfPrivateTrackCollection(trackCollection, imageShare, imageDelete);
                        else
                            ShowActionSheetIfSharedTrackCollection(trackCollection, imageDelete);
                    }
                    else
                    {
                        Mvx.IoCProvider.Resolve<IUserDialogs>().ActionSheet(new ActionSheetConfig()
                            .SetTitle(((TrackCollection)item).Name)
                            .SetCancel(TextSource[Translations.UserDialogs_Cancel])
                    );
                    }
                    break;

                case DocumentType.Podcast:
                    var podcastId = item.Id;
                    await _navigationService.Navigate<AutomaticDownloadViewModel, int>(podcastId);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowActionSheetIfSharedTrackCollection(
            TrackCollection trackCollection,
            string imageDelete)
        {
            var userDialogs = Mvx.IoCProvider.Resolve<IUserDialogs>();

            userDialogs.ActionSheet(new ActionSheetConfig()
                .SetTitle(trackCollection.Name)
                .AddHandled(
                    TextSource[Translations.TrackCollectionViewModel_RemovePlaylist],
                    async () => await RemoveSharedPlaylist(trackCollection.Id),
                    imageDelete)
                .SetCancel(TextSource[Translations.UserDialogs_Cancel]));
        }

        private void ShowActionSheetIfPrivateTrackCollection(
            TrackCollection trackCollection,
            string imageShare,
            string imageDelete)
        {
            var imageRename = "icon_edit_static.png";
            Mvx.IoCProvider.Resolve<IUserDialogs>().ActionSheet(new ActionSheetConfig()
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
                        await _navigationService.Navigate<EditTrackCollectionViewModel, ITrackCollectionParameter>(
                            new TrackCollectionParameter(trackCollection.Id));
                    },
                    imageRename)
                .SetCancel(TextSource[Translations.UserDialogs_Cancel])
            );
        }

        protected async Task ShareTrackCollection(int trackCollectionId)
        {
            await _navigationService.Navigate<ShareTrackCollectionViewModel, ITrackCollectionParameter>(
                new TrackCollectionParameter(trackCollectionId));
        }

        protected async Task RemoveSharedPlaylist(int trackCollectionId)
        {
            await Client.TrackCollection.Unfollow(trackCollectionId);
            await CloseCommand.ExecuteAsync();
        }

        protected virtual async Task ShowTrackInfo(Track track)
        {
            if (track == null)
            {
                Mvx.IoCProvider.Resolve<IAnalytics>().LogEvent("4934 the track is null", new Dictionary<string, object> { { "CallingType", this.GetType().FullName } });
                return;
            }

            _messenger.Publish(new TogglePlayerMessage(this, false));
            await _navigationService.Navigate<TrackInfoViewModel, Track>(track);
        }

        private IMvxCommand<Document> _documentSelectedCommand;

        public IMvxCommand<Document> DocumentSelectedCommand => _documentSelectedCommand ?? (_documentSelectedCommand = new ExceptionHandlingCommand<Document>(DocumentAction));

        protected virtual async Task DocumentAction(Document item)
        {
            if (item != null)
                await DocumentAction(item, null);
        }

        /// <summary>
        /// Centralized action for what to do if you click on a document.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="list">List of tracks.</param>
        protected virtual async Task DocumentAction(Document item, IList<Track> list)
        {
            switch (item.DocumentType)
            {
                case DocumentType.Track:
                    var track = (Track)item;
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

                    await mediaPlayer.Play(list.OfType<IMediaTrack>().ToList(), track, GetType().Name);
                    break;

                case DocumentType.Album:
                    await _navigationService.Navigate<AlbumViewModel, Album>((Album)item);
                    break;

                case DocumentType.Contributor:
                    await _navigationService.Navigate<ContributorViewModel, int>(item.Id);
                    break;

                case DocumentType.TrackCollection:
                    var trackCollection = (TrackCollection)item;
                    await _navigationService.Navigate<TrackCollectionViewModel, ITrackCollectionParameter>(new TrackCollectionParameter(trackCollection.Id, trackCollection.Name));
                    break;

                case DocumentType.Podcast:
                    var podcast = (Podcast)item;
                    await _navigationService.Navigate<PodcastViewModel, Podcast>(new Podcast {Id = podcast.Id, Title = podcast.Title});
                    break;

                case DocumentType.Playlist:
                    var playlist = (Playlist)item;
                    await _navigationService.Navigate<CuratedPlaylistViewModel, Playlist>(new Playlist {Id = playlist.Id, Title = playlist.Title});
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
            return _navigationService.Navigate<TrackCollectionsAddToViewModel, TrackCollectionsAddToViewModel.Parameter>(new TrackCollectionsAddToViewModel.Parameter
            {
                DocumentId = albumId,
                DocumentType = DocumentType.Album
            });
        }
    }
}