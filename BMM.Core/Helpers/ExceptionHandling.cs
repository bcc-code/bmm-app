using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Implementations.Exceptions;
using MvvmCross;

namespace BMM.Core.Helpers
{
    public static class ExceptionHandling
    {
        public static Action Action(Func<Task> action)
        {
            return async () =>
            {
                try
                {
                    await action();
                }
                catch (Exception e)
                {
                    Mvx.IoCProvider.Resolve<IExceptionHandler>().HandleException(e);
                }
            };
        }

        /// <summary>
        /// Catches exceptions and shows a toast in case that an exception is caught
        /// </summary>
        public static ActionSheetConfig AddHandled(this ActionSheetConfig actionSheet, string text, Func<Task> action, string icon = null)
        {
            return actionSheet.Add(text, Action(action), icon);
        }
    }
}
