using BMM.Api.Framework;
using BMM.Core.Implementations.Logger;
using BMM.Core.Implementations.Security;
using Java.Lang;
using Exception = System.Exception;

namespace BMM.UI.Droid.Application.Implementations
{
    public class AndroidLogger : BaseLogger
    {
        public AndroidLogger(
            IUserStorage userStorage,
            IConnection connection) 
            : base(connection, userStorage)
        {
        }
        
        public override void Error(string tag, string message, Exception exception, bool presentedToUser = false)
        {
            var throwableException = Throwable.FromException(exception);
            base.Error(tag, message, throwableException, presentedToUser);
        }
    }
}