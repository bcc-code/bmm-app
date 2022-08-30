using BMM.Api.Framework;
using BMM.Core.Implementations.Logger;
using BMM.Core.Implementations.Security;

namespace BMM.UI.iOS.Implementations
{
    public class IosLogger : BaseLogger
    {
        public IosLogger(
            IUserStorage userStorage,
            IConnection connection)
            : base(connection, userStorage)
        {
        }
    }
}