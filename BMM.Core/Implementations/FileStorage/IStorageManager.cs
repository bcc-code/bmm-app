using System.Collections.ObjectModel;
using BMM.Core.Implementations.DownloadManager;

namespace BMM.Core.Implementations.FileStorage
{
    public interface IStorageManager
    {
        /// <summary>
        /// The list of all possible available storages. Used to select a default storage.
        /// </summary>
        /// <value>The storages.</value>
        ObservableCollection<IFileStorage> Storages { get; }


        void RefreshStorages();

        /// <summary>
        /// An indicator if the OS supports more than one path to store files at.
        /// When the system returns FALSE here, you can only use the DefaultStorage.
        /// The list of Storages will not contain any more.
        /// </summary>
        /// <value><c>true</c> if the OS supports more than one position to store the files; otherwise, <c>false</c>.</value>
        bool HasMultipleStorageSupport { get; }

        /// <summary>
        /// The storage to save files at.
        /// This decision can be changed by the user, but is the SystemDefaultStorage by default.
        /// </summary>
        /// <value>The default storage.</value>
        IFileStorage SelectedStorage { get; set; }

        /// <summary>
        /// The default location to store files. This decission is only taken by the OS.
        /// It's usually related as application documents.
        /// </summary>
        /// <value>The system default storage.</value>
        IFileStorage SystemDefaultStorage { get; }

        string GetUrlByFile(IDownloadFile file);
    }
}