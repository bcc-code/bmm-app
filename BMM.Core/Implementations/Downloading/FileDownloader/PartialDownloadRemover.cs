using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Startup;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    /// <summary>
    /// Checks if the app was downloading a file while being killed and deletes the remaining partial file.
    /// It also triggers a synchronization of offline files to download new or missing files in the case the user is logged in.
    /// </summary>
    public class PartialDownloadRemover : IDelayedStartupTask
    {
        private readonly IBlobCache _localStorage;
        private readonly IStorageManager _storageManager;
        private readonly IGlobalMediaDownloader _mediaDownloader;
        private readonly IUserAuthChecker _userAuthChecker;

        public PartialDownloadRemover(IBlobCache localStorage, IStorageManager storageManager, IGlobalMediaDownloader mediaDownloader, IUserAuthChecker userAuthChecker)
        {
            _localStorage = localStorage;
            _storageManager = storageManager;
            _mediaDownloader = mediaDownloader;
            _userAuthChecker = userAuthChecker;
        }

        public async Task RunAfterStartup()
        {
            var storedDownloadable = await _localStorage.GetOrCreateObject<PersistedDownloadable>(StorageKeys.CurrentDownload, () => null);
            if (storedDownloadable != null)
            {
                _storageManager.SelectedStorage.DeleteFile(storedDownloadable);
                await _localStorage.InsertObject<PersistedDownloadable>(StorageKeys.CurrentDownload, null);
            }

            if (await _userAuthChecker.IsUserAuthenticated())
            {
                await _mediaDownloader.SynchronizeOfflineTracks();
            }
        }
    }
}