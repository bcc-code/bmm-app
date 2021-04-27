using BMM.Core.Implementations.FileStorage;
using System;
using System.Collections.ObjectModel;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.DownloadManager;

namespace BMM.UI.iOS
{
    public class StorageManager : IStorageManager
    {
        private readonly IAnalytics _analytics;

        public StorageManager(IAnalytics analytics)
        {
            _analytics = analytics;
        }

        public ObservableCollection<IFileStorage> Storages => new ObservableCollection<IFileStorage> { SelectedStorage };

        public void RefreshStorages()
        {
        }

        public bool HasMultipleStorageSupport => false;

        public IFileStorage SelectedStorage
        {
            get => SystemDefaultStorage;
            set => throw new NotImplementedException();
        }

        public IFileStorage SystemDefaultStorage => new TouchFileStorage(_analytics);

        public string GetUrlByFile(IDownloadFile file)
        {
            return SelectedStorage.GetUrlByFile(file);
        }

        public string GetUrlByFile(TrackMediaFile trackMediaFile)
        {
            return SelectedStorage.GetUrlByFile(trackMediaFile);
        }
    }
}