using System;
using System.Reactive.Linq;
using MvvmCross;

namespace BMM.Core.Implementations.Exceptions
{
    public static class IObservable
    {
        /// <summary>
        /// Handles all exceptions of type TException from the observable sequence.
        /// </summary>
        /// <returns>An observable sequence without all the exceptions of type TException</returns>
        /// <param name="observable">The observable to handle the exceptions of type TException for.</param>
        /// <typeparam name="TException">The type of exception to handle</typeparam>
        /// <typeparam name="TResult">The value that is emitted</typeparam>
        public static IObservable<TResult> HandleExceptions<TException, TResult>(this IObservable<TResult> observable)
            where TException: Exception
        {
            var exceptionHandler = Mvx.IoCProvider.Resolve<IExceptionHandler>();

            return observable.Catch((TException ex) => {
                exceptionHandler.HandleException(ex);
                return Observable.Empty<TResult>();
            });
        }
    }
}