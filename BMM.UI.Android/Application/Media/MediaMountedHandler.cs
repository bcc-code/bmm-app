using Android.Content;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.FileStorage;

namespace BMM.UI.Droid.Application.Media
{
    public class MediaMountedHandler
    {
        private readonly IStorageManager _storageManager;

        private readonly ISettingsStorage _settingsStorage;

        public MediaMountedHandler(IStorageManager storageManager, ISettingsStorage settingsStorage)
        {
            _storageManager = storageManager;
            _settingsStorage = settingsStorage;
        }

        public void MediaUnmounted(Context context, Intent intent)
        {
            if (_storageManager.HasMultipleStorageSupport)
            {
                _storageManager.SelectedStorage = _storageManager.SystemDefaultStorage;
                _storageManager.RefreshStorages();
            }
        }

        public void MediaMounted(Context context, Intent intent)
        {
            _storageManager.RefreshStorages();
        }
     }
}