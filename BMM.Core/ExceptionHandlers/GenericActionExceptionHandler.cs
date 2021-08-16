using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.ExceptionHandlers.Interfaces;
using BMM.Core.Implementations.Exceptions;

namespace BMM.Core.ExceptionHandlers
{
    public class GenericActionExceptionHandler : IGenericActionExceptionHandler
    {
        private readonly IExceptionHandler _exceptionHandler;

        public GenericActionExceptionHandler(IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        public Task HandleException(Exception ex)
        {
            _exceptionHandler.HandleException(ex);
            return Task.CompletedTask;
        }

        public IEnumerable<Type> GetTriggeringExceptionTypes()
        {
            yield return typeof(Exception);
        }
    }
}