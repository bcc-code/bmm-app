using System.Collections.ObjectModel;
using Android.Content;
using AndroidX.Core.Content;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.FileStorage;
using Exception = System.Exception;

namespace BMM.UI.Droid.Application.Implementations.FileStorage
{
    public class StorageManager : IStorageManager
    {
        private readonly IAnalytics _analytics;
        private readonly Context _context;
        private int _selectedIndex = 0;

        public StorageManager(ISettingsStorage settingsStorage, IAnalytics analytics)
        {
            _context = Android.App.Application.Context;
            Storages = new ExtendedMvxObservableCollection<IFileStorage>();
            _analytics = analytics;

            FillUpStorages();
            SelectDefaultSource(settingsStorage.UseExternalStorage());
        }

        public ObservableCollection<IFileStorage> Storages { get; }

        public void RefreshStorages()
        {
            ((ExtendedMvxObservableCollection<IFileStorage>)Storages).ClearWithoutTriggeringEvents();
            FillUpStorages();
        }

        public bool HasMultipleStorageSupport => true;

        public IFileStorage SelectedStorage
        {
            get
            {
                if (!Storages.Any())
                    throw new Exception("trying to access StorageManager before it's initialized");

                return Storages[_selectedIndex];
            }
            set
            {
                var selectedIndex = _selectedIndex;
                try
                {
                    _selectedIndex = Storages.IndexOf(value);
                }
                catch
                {
                    _selectedIndex = selectedIndex;
                }
            }
        }

        public IFileStorage SystemDefaultStorage => SelectInternalStorage();

        public string GetUrlByFile(IDownloadFile file)
        {
            return SelectedStorage.GetUrlByFile(file);
        }

        public string GetUrlByFile(TrackMediaFile trackMediaFile)
        {
            return SelectedStorage.GetUrlByFile(trackMediaFile);
        }

        private IFileStorage SelectExternalStorage()
        {
            return Storages.FirstOrDefault(x => x.StorageKind == StorageKind.External);
        }

        private IFileStorage SelectInternalStorage()
        {
            return Storages.FirstOrDefault(x => x.StorageKind == StorageKind.Internal);
        }

        // Using async void should be fine because the constructor is called on the main thread. It's not a nice solution though.
        private async void SelectDefaultSource(Task<bool> useExternalStorage)
        {
            bool result = await useExternalStorage;
            var externalStorage = SelectExternalStorage();
            if (result && externalStorage != null)
            {
                _selectedIndex = Storages.IndexOf(externalStorage);
            }
        }

        private void FillUpStorages()
        {
            var directories = ContextCompat.GetExternalFilesDirs(_context, null);

            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i] != null && directories[i].TotalSpace > 0)
                {
                    var storageKind = i > 0 ? StorageKind.External : StorageKind.Internal;
                    Storages.Add(new AndroidFileStorage(directories[i], storageKind, _analytics));
                }
            }
        }
    }
}