using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Messages;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class StorageManagementViewModel : BaseViewModel
    {
        private readonly IStorageManager _storageManager;
        private readonly ISettingsStorage _settingsStorage;
        private readonly IUserDialogs _userDialog;
        private readonly IAnalytics _analytics;
        private IFileStorage _selectedStorage;

        private IMvxAsyncCommand<IFileStorage> _storageSelectedCommand;

        public StorageManagementViewModel(
            IStorageManager storageManager,
            ISettingsStorage settingsStorage,
            IUserDialogs userDialog,
            IAnalytics analytics)
        {
            _storageManager = storageManager;
            _settingsStorage = settingsStorage;
            _userDialog = userDialog;
            _analytics = analytics;
            _selectedStorage = _storageManager.SelectedStorage;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            _storageManager.Storages.CollectionChanged += OnStoragesCollectionChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            _storageManager.Storages.CollectionChanged -= OnStoragesCollectionChanged;
        }

        private void OnStoragesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Storages);
            _selectedStorage = _storageManager.SelectedStorage;
            RaisePropertyChanged(() => SelectedStorage);
        }

        public ObservableCollection<IFileStorage> Storages => _storageManager.Storages;

        public IFileStorage SelectedStorage
        {
            get => _selectedStorage;
            set
            {
                _selectedStorage = value;
                RaisePropertyChanged(() => SelectedStorage);
                Messenger.Publish(new SelectedStorageChangedMessage(this, value));
            }
        }

        public IMvxAsyncCommand<IFileStorage> StorageSelectedCommand
        {
            get
            {
                _storageSelectedCommand = _storageSelectedCommand ?? new ExceptionHandlingCommand<IFileStorage>(StorageSelected);
                return _storageSelectedCommand;
            }
        }

        private async Task StorageSelected(IFileStorage fileStorage)
        {
            if (SelectedStorage != fileStorage)
            {
                bool result = await _userDialog.ConfirmAsync(TextSource[Translations.StorageManagementViewModel_RemoveAllFiles]);
                if (result)
                {
                    await _settingsStorage.SetStorageLocation(fileStorage.StorageKind == StorageKind.External);
                    _storageManager.SelectedStorage = fileStorage;
                    SelectedStorage = fileStorage;
                    var count = _storageManager.SelectedStorage.RemoveAllDownloadedFilesAndGetCount();

                    _analytics.LogEvent(
                        "Removed all tracks due storage change",
                        new Dictionary<string, object>
                        {
                            {"tracksDeleted", count}
                        });
                }
            }
        }
    }
}