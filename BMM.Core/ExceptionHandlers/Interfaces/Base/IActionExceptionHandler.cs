using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMM.Core.ExceptionHandlers.Interfaces.Base
{
    public interface IActionExceptionHandler
    {
        Task HandleException(Exception ex);

        IEnumerable<Type> GetTriggeringExceptionTypes();
    }
}