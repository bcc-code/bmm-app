using BMM.Api.Framework;
using BMM.Core.Helpers;

namespace BMM.Core.Implementations
{
    public class BmmVersionProvider : IBmmVersionProvider
    {
        public string BmmVersion => GlobalConstants.AppVersion;
    }
}