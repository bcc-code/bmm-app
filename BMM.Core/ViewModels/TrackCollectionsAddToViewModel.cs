using System;
using Acr.UserDialogs;
using BMM.Api.Implementation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Implementations.UI;
using BMM.Core.Implementations.TrackCollections.Exceptions;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class TrackCollectionsAddToViewModel : ContentBaseViewModel, IMvxViewModel<TrackCollectionsAddToViewModel.Parameter>
    {
        private readonly ITrackCollectionManager _trackCollectionManager;
        private readonly IUserDialogs _userDialogs;
        private readonly IToastDisplayer _toastDisplayer;
        private int _documentId;
        private DocumentType _documentType;
        private readonly ILogger _logger;

        public TrackCollectionsAddToViewModel(IOfflineTrackCollectionStorage downloader, ITrackCollectionManager trackCollectionManager, IUserDialogs userDialogs,
            IToastDisplayer toastDisplayer, IStorageManager storageManager, ILogger logger)
            : base(downloader, storageManager)
        {
            _trackCollectionManager = trackCollectionManager;
            _userDialogs = userDialogs;
            _toastDisplayer = toastDisplayer;
            _logger = logger;
        }

        public void Prepare(Parameter document)
        {
            _documentId = document.DocumentId;
            _documentType = document.DocumentType;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var items = await base.LoadItems(policy);
            return items
                .OfType<TrackCollection>()
                .Where(t => t.CanEdit);
        }

        protected override async Task DocumentAction(Document item, IList<Track> list)
        {
            var targetTrackCollection = (TrackCollection)item;
            try
            {
                await _trackCollectionManager.AddToTrackCollection(targetTrackCollection, _documentId, _documentType);
                await _toastDisplayer.Success(TextSource[Translations.TrackCollectionsAddToViewModel_TrackAddedToTrackCollection]);
            }
            catch (UnsupportedDocumentTypeException)
            {
                await _userDialogs.AlertAsync(TextSource.GetText(Translations.TrackCollectionsAddToViewModel_FailedToAddUnknownType, _documentType.ToString()));
            }
            catch (AlbumAlreadyInTrackCollectionException)
            {
                await _userDialogs.AlertAsync(TextSource.GetText(Translations.TrackCollectionsAddToViewModel_AlbumFailedToAddAlreadyExists, targetTrackCollection.Name));
            }
            catch (TrackAlreadyInTrackCollectionException)
            {
                await _userDialogs.AlertAsync(TextSource.GetText(Translations.TrackCollectionsAddToViewModel_TrackAlreadyExistInTrackCollection, targetTrackCollection.Name));
            }
            catch (BadRequestException ex)
            {
                await _userDialogs.AlertAsync(TextSource.GetText(Translations.TrackCollectionsAddToViewModel_FailedToAdd, targetTrackCollection.Name));
                _logger.Error("TrackOrAlbumAddedToTrackCollection", "Bad request", ex);
            }
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync(TextSource[Translations.Global_UnexpectedError]);
                _logger.Error("TrackOrAlbumAddedToTrackCollection", "Unexpected error", ex);
            }

            await NavigationService.Close(this);
        }

        public class Parameter
        {
            /// <summary>
            /// Id of the track or album
            /// </summary>
            public int DocumentId { get; set; }

            /// <summary>
            /// Should be either Track or Album
            /// </summary>
            public DocumentType DocumentType { get; set; }
        }
    }
}