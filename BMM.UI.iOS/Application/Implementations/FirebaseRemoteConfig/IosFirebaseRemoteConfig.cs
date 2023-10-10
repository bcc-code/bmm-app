using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using Firebase.RemoteConfig;

namespace BMM.UI.iOS.Implementations
{
    public class IosFirebaseRemoteConfig : IPlatformSpecificRemoteConfig
    {
        private readonly RemoteConfig _firebaseRemote;

        public IosFirebaseRemoteConfig()
        {
            _firebaseRemote = RemoteConfig.SharedInstance;

            var defaults = FirebaseRemoteConfig.Defaults.ToDictionary(pair => pair.Key as object, pair => pair.Value as object);
            _firebaseRemote.SetDefaults(defaults);
        }

        public bool GetBoolValue(string id)
        {
            return _firebaseRemote.GetConfigValue(id).BoolValue;
        }

        public async Task UpdateValuesFromFirebaseRemoteConfig()
        {
            await _firebaseRemote.FetchAndActivateAsync();
        }

        public int GetIntValue(string id)
        {
            return _firebaseRemote.GetConfigValue(id).NumberValue.Int32Value;
        }

        public string GetStringValue(string id)
        {
            return _firebaseRemote.GetConfigValue(id).StringValue;
        }
    }
}