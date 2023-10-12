using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.Implementations.FirebaseRemoteConfig
{
    public class AndroidFirebaseRemoteConfig : IPlatformSpecificRemoteConfig
    {
        private readonly Firebase.RemoteConfig.FirebaseRemoteConfig _firebaseRemote;

        public AndroidFirebaseRemoteConfig()
        {
            _firebaseRemote = Firebase.RemoteConfig.FirebaseRemoteConfig.Instance;

            var defaults = Core.Implementations.FirebaseRemoteConfig.FirebaseRemoteConfig.Defaults.ToDictionary(pair => pair.Key, pair => new Java.Lang.String(pair.Value) as Object);
            _firebaseRemote.SetDefaultsAsync(defaults);
        }

        public bool GetBoolValue(string id)
        {
            return _firebaseRemote.GetBoolean(id);
        }

        public int GetIntValue(string id)
        {
            return (int) _firebaseRemote.GetLong(id);
        }

        public async Task UpdateValuesFromFirebaseRemoteConfig()
        {
            await _firebaseRemote.FetchAsync(FirebaseConfigConstants.MinimumFetchIntervalInSeconds);
            _firebaseRemote.Activate();
        }

        public string GetStringValue(string id)
        {
            return _firebaseRemote.GetString(id);
        }
    }
}