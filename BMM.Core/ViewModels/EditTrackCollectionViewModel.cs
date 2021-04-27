using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class EditTrackCollectionViewModel : DocumentsViewModel, IMvxViewModel<EditTrackCollectionParameters>
    {
        private readonly IUserDialogs _userDialogs;
        private readonly ILogger _logger;
        private int _trackCollectionId;
        private TrackCollection _trackCollection;
        private string _trackCollectionTitle;
        private bool _hasChanges;

        public string TrackCollectionTitle { get => _trackCollectionTitle; set => SetProperty(ref _trackCollectionTitle, value); }

        public bool HasChanges { get => _hasChanges; set => SetProperty(ref _hasChanges, value); }

        public IMvxAsyncCommand SaveAndCloseCommand { get; }

        public IMvxAsyncCommand DiscardAndCloseCommand { get; }

        public EditTrackCollectionViewModel(IUserDialogs userDialogs, ILogger logger)
        {
            _userDialogs = userDialogs;
            _logger = logger;
            SaveAndCloseCommand = new MvxAsyncCommand(SaveAndClose);
            DiscardAndCloseCommand = new ExceptionHandlingCommand(CloseWithDiscardIfNeeded);
            PropertyChanged += OnPropertyChanged;
            Documents.CollectionChanged += DocumentsOnCollectionChanged;
        }


        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var trackCollection = await Client.TrackCollection.GetById(_trackCollectionId, policy);
            _trackCollection = trackCollection;
            TrackCollectionTitle = trackCollection.Name;
            return trackCollection.Tracks;
        }

        public void Prepare(EditTrackCollectionParameters parameter)
        {
            _trackCollectionId = parameter.TrackCollectionId;
        }

        private async Task CloseWithDiscardIfNeeded()
        {
            if (HasChanges)
            {
                var shouldDiscard = await _userDialogs.ConfirmAsync(
                    TextSource.GetText("DiscardChangesMessage"),
                    TextSource.GetText("DiscardChangesTitle"),
                    TextSource.GetText("DiscardChanges"),
                    TextSource.GetText("KeepEditing"));
                if (!shouldDiscard)
                    return;
            }

            await _navigationService.Close(this);
        }

        private async Task SaveAndClose()
        {
            if (_trackCollection == null)
                return;

            if (string.IsNullOrWhiteSpace(TrackCollectionTitle))
            {
                await _userDialogs.AlertAsync(TextSource.GetText("EmptyName"));
                return;
            }

            _trackCollection.Tracks = Documents.OfType<Track>().ToList();
            _trackCollection.Name = TrackCollectionTitle;
            try
            {
                await Client.TrackCollection.Save(_trackCollection);
                _messenger.Publish(new TrackCollectionOrderChangedMessage(this) {TrackCollection = _trackCollection});
                await _navigationService.Close(this);
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(EditTrackCollectionViewModel), "Updating TrackCollection failed", ex);
                await _userDialogs.AlertAsync(TextSource.GetText("SaveFailure"));
            }
        }

        private void DocumentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CheckForChanges();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TrackCollectionTitle))
                CheckForChanges();
        }

        private void CheckForChanges()
        {
            var docIds = Documents.Select(x => x.Id);
            var collectionIds = _trackCollection.Tracks.Select(x => x.Id);

            var docsHaveChanges = !docIds.SequenceEqual(collectionIds);
            var titleHasChanges = _trackCollection.Name != TrackCollectionTitle;

            HasChanges = docsHaveChanges || titleHasChanges;
        }
    }

    public class TrackCollectionOrderChangedMessage : MvxMessage
    {
        public TrackCollectionOrderChangedMessage(object sender) : base(sender)
        { }

        public TrackCollection TrackCollection { get; set; }
    }

    public struct EditTrackCollectionParameters
    {
        public int TrackCollectionId { get; set; }
    }
}