using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.FirebaseRemoteConfig;

namespace BMM.Core.Implementations.Networking
{
    public class ExperimentIdHeaderProvider : IHeaderProvider
    {
        private readonly IFirebaseRemoteConfig _config;

        public ExperimentIdHeaderProvider(IFirebaseRemoteConfig config)
        {
            _config = config;
        }

        public async Task<KeyValuePair<string, string>?> GetHeader()
        {
            if (!string.IsNullOrEmpty(_config.ExperimentId))
            {
                return new KeyValuePair<string, string>("ExperimentId", _config.ExperimentId);
            }

            return null;
        }
    }
}