using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.UI;

namespace BMM.UI.iOS.UI
{
    public class DroidExceptionHandlingUserDialogs : ExceptionHandlingUserDialogs
    {
        public DroidExceptionHandlingUserDialogs(
            IUserDialogsFactory userDialogsFactory,
            IExceptionHandler exceptionHandler)
            : base(userDialogsFactory.Create(), exceptionHandler)
        {
        }
    }
}