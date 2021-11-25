using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.UI;

namespace BMM.UI.iOS.UI
{
    public class iOSExceptionHandlingUserDialogs : ExceptionHandlingUserDialogs
    {
        public iOSExceptionHandlingUserDialogs(IUserDialogsFactory userDialogsFactory, IExceptionHandler exceptionHandler)
            : base(userDialogsFactory.Create(), exceptionHandler)
        {
        }
    }
}