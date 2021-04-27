using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Core.Implementations.Startup;

namespace BMM.Core.Implementations.FirebaseRemoteConfig
{
    public class FirebaseConfigUpdater : IDelayedStartupTask
    {
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
        private readonly IConnection _connection;

        public FirebaseConfigUpdater(IFirebaseRemoteConfig firebaseRemoteConfig, IConnection connection)
        {
            _firebaseRemoteConfig = firebaseRemoteConfig;
            _connection = connection;
        }

        public async Task RunAfterStartup()
        {
            var connectionStatus = _connection.GetStatus();

            if (connectionStatus == ConnectionStatus.Online)
            {
                await _firebaseRemoteConfig.UpdateValuesFromFirebaseRemoteConfig();
            }
        }
    }
}
